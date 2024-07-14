using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using CoMon.Assets;
using CoMon.Packages;
using CoMon.Packages.Dtos;
using CoMon.Packages.Settings;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Assistant.Plugins
{
    [Description("Plugin to get information about packages and manage them. Packages belong to an asset.")]
    public class PackagePlugin(IRepository<Asset, long> _assetRepository, IRepository<Package, long> _packageRepository,
        IRepository<Status, long> _statusRepository, IObjectMapper _mapper)
    {
        [KernelFunction, Description("Search for packages by name.")]
        public async Task<List<PackageDto>> SearchPackages(string name)
        {
            var packages = await _packageRepository
                .GetAll()
                .Where(p => p.Name.ToLower().Contains(name.ToLower()))
                .Include(p => p.Asset)
                .Include(p => p.PingPackageSettings)
                .Include(p => p.HttpPackageSettings)
                .Include(p => p.RtspPackageSettings)
                .Include(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<PackageDto>>(packages);
        }

        [KernelFunction, Description("Get a package by its ID along with its asset and the latest status of the package.")]
        public async Task<PackageDto> GetPackageById(int id)
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

        [KernelFunction, Description("Gets the healthy, warning and alert durations and percentages for each package from the given last hours.")]
        public async Task<List<PackageStatisticDto>> GetStatistics(int hours)
        {
            return await Helpers.GetStatistics(_packageRepository, _statusRepository, _mapper, hours);
        }

        [KernelFunction, Description("Gets the healthy, warning and alert durations and percentages for the package with given id from the given last hours.")]
        public async Task<PackageStatisticDto> GetStatistic(long packageId, int hours)
        {
            return await Helpers.GetStatistic(_packageRepository, _statusRepository, _mapper, packageId, hours);
        }

        [KernelFunction, Description("Update the name of a package.")]
        public async Task UpdateName(int id, string name)
        {
            var package = await _packageRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("Package not found.");

            package.Name = name;
            await _packageRepository.UpdateAsync(package);
        }

        [KernelFunction, Description("Create a new PING package.")]
        public async Task<long> CreatePingPackage(string name, long assetId, string host, int cycleSeconds = 60)
        {
            var input = new CreatePackageDto()
            {
                Name = name,
                AssetId = assetId,
                Type = PackageType.Ping,
                PingPackageSettings = new PingPackageSettingsDto()
                {
                    Host = host,
                    CycleSeconds = cycleSeconds
                }
            };
            Helpers.ValidateSettings(input);

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
                    TriggerCause = TriggerCause.Initialized,
                    Messages = ["The package was initialized."]
                }
            ];

            return await _packageRepository.InsertAndGetIdAsync(package);
        }

        [KernelFunction, Description("Create a new HTTP package.")]
        public async Task<long> CreateHttpPackage(string name, long assetId, string url, HttpPackageMethod method, int cycleSeconds = 60)
        {
            var input = new CreatePackageDto()
            {
                Name = name,
                AssetId = assetId,
                Type = PackageType.Http,
                HttpPackageSettings = new HttpPackageSettingsDto()
                {
                    Url = url,
                    Method = method,
                    CycleSeconds = cycleSeconds
                }
            };
            Helpers.ValidateSettings(input);

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
                    TriggerCause = TriggerCause.Initialized,
                    Messages = ["The package was initialized."]
                }
            ];

            return await _packageRepository.InsertAndGetIdAsync(package);
        }

        [KernelFunction, Description("Create a new RTSP package.")]
        public async Task<long> CreateRtspPackage(string name, long assetId, string url, RtspPackageMethod method, int cycleSeconds = 60)
        {
            var input = new CreatePackageDto()
            {
                Name = name,
                AssetId = assetId,
                Type = PackageType.Rtsp,
                RtspPackageSettings = new RtspPackageSettingsDto()
                {
                    Url = url,
                    Method = method,
                    CycleSeconds = cycleSeconds
                }
            };
            Helpers.ValidateSettings(input);

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
                    TriggerCause = TriggerCause.Initialized,
                    Messages = ["The package was initialized."]
                }
            ];

            return await _packageRepository.InsertAndGetIdAsync(package);
        }
    }
}
