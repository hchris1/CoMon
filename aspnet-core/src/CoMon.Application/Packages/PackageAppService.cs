using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Validation;
using CoMon.Assets;
using CoMon.Packages.Dtos;
using CoMon.Statuses;
using CoMon.Statuses.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Packages
{
    public class PackageAppService(IRepository<Asset, long> assetRepository,
        IRepository<Package, long> packageRepository, IObjectMapper mapper, IRepository<Status, long> statusRepository)
        : CoMonAppServiceBase, IPackageAppService
    {
        private readonly IRepository<Asset, long> _assetRepository = assetRepository;
        private readonly IRepository<Package, long> _packageRepository = packageRepository;
        private readonly IRepository<Status, long> _statusRepository = statusRepository;
        private readonly IObjectMapper _mapper = mapper;

        public async Task<PackageDto> Get(long id)
        {
            return _mapper.Map<PackageDto>(
                await _packageRepository
                .GetAll()
                .Include(p => p.Asset)
                .Include(p => p.PingPackageSettings)
                .Include(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Package not found")
                );
        }

        public async Task<PackagePreviewDto> GetPreview(long id)
        {
            return _mapper.Map<PackagePreviewDto>(
                await _packageRepository
                .GetAll()
                .Include(p => p.Asset)
                .ThenInclude(a => a.Group.Parent.Parent)
                .Include(p => p.PingPackageSettings)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Package not found")
                );
        }

        public async Task<long> Create(CreatePackageDto input)
        {
            if (input.Type == PackageType.Ping && input.PingPackageSettings == null)
                throw new AbpValidationException("PingPackageSettings may not be null.");

            var asset = _assetRepository
                .GetAll()
                .Where(a => a.Id == input.AssetId)
                .FirstOrDefault()
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

        public async Task Update(UpdatePackageDto input)
        {
            var package = _mapper.Map<Package>(input);

            await _packageRepository.UpdateAsync(package);
        }

        public async Task Delete(long id)
        {
            await _packageRepository.DeleteAsync(id);
        }

        private static DateTime FloorToPreviousHour(DateTime dateTime)
        {
            // Floor the original DateTime
            DateTime flooredDateTime = new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);

            return flooredDateTime;
        }

        private static DateTime FloorToPreviousDay(DateTime dateTime)
        {
            // Floor the original DateTime
            DateTime flooredDateTime = new(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);

            return flooredDateTime;
        }

        public async Task<List<PackageStatusCountDto>> GetPackageStatusUpdateBuckets(long packageId, int numOfHours = 24, bool useHourBuckets = true)
        {
            DateTime endDate = FloorToPreviousHour(DateTime.UtcNow);
            DateTime startDate = DateTime.UtcNow.AddHours(-numOfHours);


            // Generate a list of all dates within the range with 1-hour buckets
            List<DateTime> allDates;
            if (useHourBuckets)
                allDates = Enumerable.Range(0, Convert.ToInt32((endDate - startDate).TotalHours) + 3)
                .Select(offset => FloorToPreviousHour(startDate).AddHours(offset))
                .ToList();
            else
                allDates = Enumerable.Range(0, (endDate - startDate).Days + 2)
                .Select(offset => startDate.AddDays(offset).Date)
                .ToList();

            var statusCounts = await _statusRepository
                .GetAll()
                .Where(s => s.Time >= startDate)
                .Where(s => s.Package.Id == packageId)
                .GroupBy(s => new { s.Time.Date, s.Time.Hour })
                .Select(g => new PackageStatusCountDto
                {
                    Date = useHourBuckets ? g.Key.Date.AddHours(g.Key.Hour + 1) : g.Key.Date,
                    HealthyCount = g.Count(s => s.Criticality == Criticality.Healthy),
                    WarningCount = g.Count(s => s.Criticality == Criticality.Warning),
                    AlertCount = g.Count(s => s.Criticality == Criticality.Alert)
                })
                .ToListAsync();

            // Left join to include dates with no statuses
            return
            [
                .. allDates
                .GroupJoin(statusCounts, d => d, sc => sc.Date, (date, counts) => new
                {
                    Date = date,
                    Counts = counts.FirstOrDefault() ?? new PackageStatusCountDto { Date = date }
                })
                .Select(x => x.Counts)
                .OrderBy(x => x.Date)
            ];
        }

        public async Task<List<PackageStatusCountDto>> GetPackageStatusChangeBuckets(long packageId, int numOfHours = 24, bool useHourBuckets = true)
        {
            DateTime endDate = FloorToPreviousHour(DateTime.UtcNow);
            DateTime startDate = DateTime.UtcNow.AddHours(-numOfHours);


            // Generate a list of all dates within the range with 1-hour buckets
            List<DateTime> allDates;
            if (useHourBuckets)
                allDates = Enumerable.Range(0, Convert.ToInt32((endDate - startDate).TotalHours) + 3)
                .Select(offset => FloorToPreviousHour(startDate).AddHours(offset))
                .ToList();
            else
                allDates = Enumerable.Range(0, (endDate - startDate).Days + 2)
                .Select(offset => startDate.AddDays(offset).Date)
                .ToList();

            var statusCounts = await _statusRepository
                .GetAll()
                .Where(s => s.Time >= startDate)
                .Where(s => s.Package.Id == packageId)
                .Select(s => new
                {
                    Previous = s,
                    Next = _statusRepository
                                .GetAll()
                                .Where(s => s.Package.Id == packageId)
                                .Where(s2 => s2.Time > s.Time)
                                .OrderBy(s2 => s2.Time)
                                .FirstOrDefault()
                })
                .Where(g => g.Next != null)
                .Where(g => g.Previous.Criticality != g.Next.Criticality)
                .GroupBy(s => new { s.Next.Time.Date, s.Next.Time.Hour })
                .Select(g => new PackageStatusCountDto
                {
                    Date = useHourBuckets ? g.Key.Date.AddHours(g.Key.Hour + 1) : g.Key.Date,
                    HealthyCount = g.Count(s => s.Next.Criticality == Criticality.Healthy),
                    WarningCount = g.Count(s => s.Next.Criticality == Criticality.Warning),
                    AlertCount = g.Count(s => s.Next.Criticality == Criticality.Alert)
                })
                .ToListAsync();

            // Left join to include dates with no statuses
            return
            [
                .. allDates
                .GroupJoin(statusCounts, d => d, sc => sc.Date, (date, counts) => new
                {
                    Date = date,
                    Counts = counts.FirstOrDefault() ?? new PackageStatusCountDto { Date = date }
                })
                .Select(x => x.Counts)
                .OrderBy(x => x.Date)
            ];
        }

        public async Task<List<KPIDto>> GetStatusUpdateKPIs(long packageId, int numOfHours)
        {
            var statusUpdateCounts = await _statusRepository
                .GetAll()
                .Where(s => s.Package.Id == packageId)
                .Where(s => s.Time > DateTime.UtcNow - TimeSpan.FromHours(numOfHours))
                .GroupBy(s => s.Criticality)
                .Select(g => new
                {
                    criticality = g.Key,
                    count = g.Count()
                }).ToListAsync();

            return
            [
                new KPIDto()
                {
                    Name = "🟢",
                    Unit = "Updates",
                    Value = statusUpdateCounts.Where(g => g.criticality == Criticality.Healthy).SingleOrDefault()?.count ?? 0,
                },
                new KPIDto()
                {
                    Name = "🟡",
                    Unit = "Updates",
                    Value = statusUpdateCounts.Where(g => g.criticality == Criticality.Warning).SingleOrDefault()?.count ?? 0,
                },
                new KPIDto()
                {
                    Name = "🔴",
                    Unit = "Updates",
                    Value = statusUpdateCounts.Where(g => g.criticality == Criticality.Alert).SingleOrDefault()?.count ?? 0,
                }
            ];
        }

        public async Task<List<KPIDto>> GetStatusChangeKPIs(long packageId, int numOfHours)
        {
            var changedStatuses = await _statusRepository
                .GetAll()
                .Where(s => s.Package.Id == packageId)
                .Where(s => s.Time > DateTime.UtcNow - TimeSpan.FromHours(numOfHours))
                .Select(s => new
                {
                    Previous = s,
                    Next = _statusRepository
                                .GetAll()
                                .Where(s => s.Package.Id == packageId)
                                .Where(s2 => s2.Time > s.Time)
                                .OrderBy(s2 => s2.Time)
                                .FirstOrDefault()
                })
                .Where(g => g.Previous.Criticality != g.Next.Criticality)
                .GroupBy(g => g.Next.Criticality)
                .Select(g => new
                {
                    criticality = g.Key,
                    count = g.Count()
                }).ToListAsync();

            return
            [
                new KPIDto()
                {
                    Name = "🟢",
                    Unit = "Changes",
                    Value = changedStatuses.Where(g => g.criticality == Criticality.Healthy).SingleOrDefault()?.count ?? 0,
                },
                new KPIDto()
                {
                    Name = "🟡",
                    Unit = "Changes",
                    Value = changedStatuses.Where(g => g.criticality == Criticality.Warning).SingleOrDefault()?.count ?? 0,
                },
                new KPIDto()
                {
                    Name = "🔴",
                    Unit = "Changes",
                    Value = changedStatuses.Where(g => g.criticality == Criticality.Alert).SingleOrDefault()?.count ?? 0,
                }
            ];
        }
    }
}
