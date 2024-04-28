using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using CoMon.Assets;
using CoMon.Packages.Dtos;
using CoMon.Packages.Workers;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Packages
{
    [AbpAuthorize]
    public class PackageAppService(IRepository<Asset, long> assetRepository, IRepository<Package, long> packageRepository,
        IObjectMapper mapper, IRepository<Status, long> statusRepository, RtspWorker rtspWorker, HttpWorker httpWorker, PingWorker pingWorker) : CoMonAppServiceBase
    {
        private readonly IRepository<Asset, long> _assetRepository = assetRepository;
        private readonly IRepository<Package, long> _packageRepository = packageRepository;
        private readonly IRepository<Status, long> _statusRepository = statusRepository;
        private readonly IObjectMapper _mapper = mapper;

        private readonly PingWorker _pingWorker = pingWorker;
        private readonly HttpWorker _httpWorker = httpWorker;
        private readonly RtspWorker _rtspWorker = rtspWorker;

        public async Task<PackageDto> Get(long id)
        {
            return _mapper.Map<PackageDto>(
                await _packageRepository
                .GetAll()
                .Include(p => p.Asset)
                .Include(p => p.PingPackageSettings)
                .Include(p => p.HttpPackageSettings)
                .Include(p => p.RtspPackageSettings)
                .Include(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .Where(p => p.Id == id)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Package not found"));
        }

        public async Task<PackagePreviewDto> GetPreview(long id)
        {
            return _mapper.Map<PackagePreviewDto>(
                await _packageRepository
                .GetAll()
                .Include(p => p.Asset)
                .ThenInclude(a => a.Group.Parent.Parent)
                .Where(p => p.Id == id)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Package not found"));
        }

        public async Task<long> Create(CreatePackageDto input)
        {
            PackageAppServiceHelper.ValidateSettings(input);

            var asset = await _assetRepository
                .GetAll()
                .Where(a => a.Id == input.AssetId)
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Asset not found.");

            var package = _mapper.Map<Package>(input);

            package.Asset = asset;
            // Add an initial status so it shows up in the UI instantly
            package.Statuses =
            [
                new Status()
                {
                    Time = DateTime.UtcNow,
                    Criticality = null,
                    Messages = ["The package was initialized."]
                }
            ];

            return await _packageRepository.InsertAndGetIdAsync(package);
        }

        public async Task<PackageStatisticDto> GetStatistic(long packageId, int hours)
        {
            var utcNow = DateTime.UtcNow;
            var analyzingDuration = TimeSpan.FromHours(hours);
            var from = utcNow - analyzingDuration;

            var entries = await PackageAppServiceHelper.GetStatusesSinceCutOff(_statusRepository, packageId, from, utcNow);

            var durationByCriticality = PackageAppServiceHelper.CalculateDurationByCriticality(entries);

            var packagePreview = await GetPreview(packageId);
            var timeline = PackageAppServiceHelper.BuildTimeline(entries, from, analyzingDuration);

            return new PackageStatisticDto(packagePreview, durationByCriticality, timeline, analyzingDuration);
        }

        public async Task<List<PackageStatisticDto>> GetStatistics(int hours)
        {
            var packageIds = await _packageRepository
                .GetAll()
                .Select(p => p.Id)
                .ToListAsync();

            return packageIds
                .Select(async id => await GetStatistic(id, hours))
                .Select(t => t.Result)
                .ToList();
        }

        public async Task Update(UpdatePackageDto input, bool enqueueCheck = false)
        {
            PackageAppServiceHelper.ValidateSettings(input);

            var package = _mapper.Map<Package>(input);

            await _packageRepository.UpdateAsync(package);

            if (enqueueCheck)
                await EnqueueCheck(package.Id);
        }

        public async Task Delete(long id)
        {
            await _packageRepository.DeleteAsync(id);
        }

        public async Task<List<PackageStatusCountDto>> GetPackageStatusUpdateBuckets(long packageId, int numOfHours = 24, bool useHourBuckets = true)
        {
            return await PackageAppServiceHelper.GetPackageStatusBuckets(_statusRepository, packageId, numOfHours, useHourBuckets, false);
        }

        public async Task<List<PackageStatusCountDto>> GetPackageStatusChangeBuckets(long packageId, int numOfHours = 24, bool useHourBuckets = true)
        {
            return await PackageAppServiceHelper.GetPackageStatusBuckets(_statusRepository, packageId, numOfHours, useHourBuckets, true);
        }

        public async Task EnqueueCheck(long packageId)
        {
            var package = await _packageRepository
                .GetAll()
                .Where(p => p.Id == packageId)
                .AsNoTracking()
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Package not found.");

            switch (package.Type)
            {
                case PackageType.Ping:
                    _pingWorker.EnqueueManualCheck(packageId);
                    break;

                case PackageType.Http:
                    _httpWorker.EnqueueManualCheck(packageId);
                    break;

                case PackageType.Rtsp:
                    _rtspWorker.EnqueueManualCheck(packageId);
                    break;

                default:
                    throw new ArgumentException("Cannot enqueue check for this package type.");
            }
        }
    }
}
