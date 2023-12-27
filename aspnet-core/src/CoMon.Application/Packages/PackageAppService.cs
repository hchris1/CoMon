using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Validation;
using CoMon.Assets;
using CoMon.Packages.Dtos;
using CoMon.Statuses;
using CoMon.Statuses.Dtos;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace CoMon.Packages
{
    [AbpAuthorize]
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
                .Include(p => p.HttpPackageSettings)
                .Include(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .Where(p => p.Id == id)
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
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Package not found")
                );
        }

        public async Task<long> Create(CreatePackageDto input)
        {
            ValidateSettings(input);

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

        private record TimeCriticality(DateTime Time, Criticality? Criticality);

        public async Task<PackageStatisticDto> GetStatistic(long packageId, int hours)
        {
            var utcNow = DateTime.UtcNow;
            var analyzingDuration = TimeSpan.FromHours(hours);
            var cutoffTime = utcNow - analyzingDuration;

            var package = await _packageRepository
                .GetAll()
                .Where(p => p.Id == packageId)
                .Include(p => p.Asset)
                .ThenInclude(a => a.Group.Parent.Parent)
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Package not found.");

            var entries = await _statusRepository
                    .GetAll()
                    .Where(s => s.PackageId == packageId && s.Time >= cutoffTime)
                    .Select(s => new TimeCriticality(s.Time, s.Criticality))
                    .ToListAsync();

            var entryBeforeCutOff = await _statusRepository
                .GetAll()
                .Where(s => s.PackageId == packageId && s.Time < cutoffTime)
                .OrderByDescending(s => s.Time)
                .Select(s => new TimeCriticality(cutoffTime, s.Criticality))
                .FirstOrDefaultAsync();

            if (entryBeforeCutOff != null)
                entries.Add(entryBeforeCutOff);

            entries = entries.OrderBy(s => s.Time).ToList();

            if (entries.Count != 0)
                entries.Add(new TimeCriticality(utcNow, entries.Last().Criticality));


            var durationByCriticality = new Dictionary<Criticality?, TimeSpan>()
            {
                [Criticality.Healthy] = TimeSpan.Zero,
                [Criticality.Warning] = TimeSpan.Zero,
                [Criticality.Alert] = TimeSpan.Zero
            };
            var nullDuration = TimeSpan.Zero;

            for (int i = 1; i < entries.Count; i++)
            {
                if (entries[i].Criticality == null)
                {
                    nullDuration += entries[i].Time - entries[i - 1].Time;
                    continue;
                }

                durationByCriticality[entries[i].Criticality] += entries[i].Time - entries[i - 1].Time;
            }

            return new PackageStatisticDto()
            {
                Package = _mapper.Map<PackagePreviewDto>(package),
                HealthyDuration = durationByCriticality[Criticality.Healthy],
                HealthyPercent = (double)durationByCriticality[Criticality.Healthy].Ticks / (double)analyzingDuration.Ticks,
                WarningDuration = durationByCriticality[Criticality.Warning],
                WarningPercent = (double)durationByCriticality[Criticality.Warning].Ticks / (double)analyzingDuration.Ticks,
                AlertDuration = durationByCriticality[Criticality.Alert],
                AlertPercent = (double)durationByCriticality[Criticality.Alert].Ticks / (double)analyzingDuration.Ticks,
                UnknownDuration = nullDuration,
                UnknownPercent = (double)nullDuration.Ticks / (double)analyzingDuration.Ticks,
                Timeline = BuildTimeline(entries, cutoffTime, analyzingDuration)
            };
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

        private List<PackageHistoryDto> BuildTimeline(List<TimeCriticality> entries, DateTime cutOffTime, TimeSpan analyzingDuration)
        {
            var result = new List<PackageHistoryDto>();
            for (int i = 1; i < entries.Count; i++)
            {
                result.Add(new PackageHistoryDto()
                {
                    Criticality = entries[i].Criticality,
                    From = entries[i - 1].Time,
                    To = entries[i].Time,
                    Percentage = 0
                });
            }

            for (int i = result.Count - 1; i > 0; i--)
            {
                if (result[i].Criticality == result[i - 1].Criticality)
                {
                    result[i - 1].To = result[i].To;
                    result.RemoveAt(i);
                }
            }

            result.ForEach(r => r.Percentage = (double)(r.To - r.From).Ticks / (double)analyzingDuration.Ticks);

            var fullPercentage = result.Sum(r => r.Percentage);

            if (fullPercentage < 1)
                result.Insert(0, new PackageHistoryDto()
                {
                    From = cutOffTime,
                    To = result.FirstOrDefault()?.From ?? cutOffTime + analyzingDuration,
                    Percentage = 1 - fullPercentage,
                    Criticality = null
                });

            return result;
        }

        private static void ValidateSettings(CreatePackageDto input)
        {
            if (input.Type == PackageType.Ping)
            {
                if (input.PingPackageSettings == null)
                    throw new AbpValidationException("PingPackageSettings may not be null.");
            }

            if (input.Type == PackageType.Http)
            {
                if (input.HttpPackageSettings == null)
                    throw new AbpValidationException("HttpPackageSettings may not be null.");

                // Validate Body
                try
                {
                    switch (input.HttpPackageSettings.Encoding)
                    {
                        case HttpPackageBodyEncoding.Json:
                            if (!string.IsNullOrWhiteSpace(input.HttpPackageSettings.Body))
                                JToken.Parse(input.HttpPackageSettings.Body); // Using Newtonsoft.Json.Linq;
                            break;

                        case HttpPackageBodyEncoding.Xml:
                            if (!string.IsNullOrWhiteSpace(input.HttpPackageSettings.Body))
                                new XmlDocument().LoadXml(input.HttpPackageSettings.Body);
                            break;

                        default:
                            throw new AbpValidationException("Invalid body encoding.");
                    }
                }
                catch (Exception ex)
                {
                    throw new AbpValidationException("Invalid body content: " + ex.Message);
                }

                // Validate Headers JSON
                try
                {
                    if (!string.IsNullOrWhiteSpace(input.HttpPackageSettings.Headers))
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(input.HttpPackageSettings.Headers);
                }
                catch (Exception ex)
                {
                    throw new AbpValidationException("Invalid headers format: " + ex.Message);
                }
            }
        }

        public async Task Update(UpdatePackageDto input)
        {
            ValidateSettings(input);

            var package = _mapper.Map<Package>(input);

            await _packageRepository.UpdateAsync(package);
        }

        public async Task Delete(long id)
        {
            await _packageRepository.DeleteAsync(id);
        }

        private static DateTime FloorToPreviousHour(DateTime dateTime)
        {
            return new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
        }

        private static DateTime FloorToPreviousDay(DateTime dateTime)
        {
            return new(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
        }

        public async Task<List<PackageStatusCountDto>> GetPackageStatusUpdateBuckets(long packageId, int numOfHours = 24, bool useHourBuckets = true)
        {
            var (startDate, endDate) = GetDateRange(numOfHours);

            var allDates = GenerateTimeBuckets(startDate, endDate, useHourBuckets);

            var query = _statusRepository
                .GetAll()
                .Where(s => s.Time >= startDate)
                .Where(s => s.Package.Id == packageId);

            List<PackageStatusCountDto> statusCounts;
            if (useHourBuckets)
                statusCounts = await query
                .GroupBy(s => new { s.Time.Date, s.Time.Hour })
                .Select(g => new PackageStatusCountDto
                {
                    Date = g.Key.Date.AddHours(g.Key.Hour + 1),
                    HealthyCount = g.Count(s => s.Criticality == Criticality.Healthy),
                    WarningCount = g.Count(s => s.Criticality == Criticality.Warning),
                    AlertCount = g.Count(s => s.Criticality == Criticality.Alert)
                })
                .ToListAsync();
            else
                statusCounts = await query
                .GroupBy(s => new { s.Time.Date })
                .Select(g => new PackageStatusCountDto
                {
                    Date = g.Key.Date,
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

        private (DateTime startDate, DateTime endDate) GetDateRange(int numOfHours)
        {
            return (DateTime.UtcNow.AddHours(-numOfHours), FloorToPreviousHour(DateTime.UtcNow).AddHours(1));
        }

        private List<DateTime> GenerateTimeBuckets(DateTime startDate, DateTime endDate, bool useHourBuckets)
        {
            if (useHourBuckets)
                return Enumerable.Range(0, Convert.ToInt32((endDate - startDate).TotalHours) + 3)
                .Select(offset => FloorToPreviousHour(startDate).AddHours(offset))
                .ToList();

            return Enumerable.Range(0, (endDate - startDate).Days + 2)
            .Select(offset => startDate.AddDays(offset).Date)
            .ToList();
        }

        public async Task<List<PackageStatusCountDto>> GetPackageStatusChangeBuckets(long packageId, int numOfHours = 24, bool useHourBuckets = true)
        {
            var (startDate, endDate) = GetDateRange(numOfHours);

            var timeBuckets = GenerateTimeBuckets(startDate, endDate, useHourBuckets);

            List<PackageStatusCountDto> statusCounts;

            var query = _statusRepository
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
                .Where(g => g.Previous.Criticality != g.Next.Criticality);

            if (useHourBuckets)
                statusCounts = await query
                .GroupBy(s => new { s.Next.Time.Date, s.Next.Time.Hour })
                .Select(g => new PackageStatusCountDto
                {
                    Date = g.Key.Date.AddHours(g.Key.Hour + 1),
                    HealthyCount = g.Count(s => s.Next.Criticality == Criticality.Healthy),
                    WarningCount = g.Count(s => s.Next.Criticality == Criticality.Warning),
                    AlertCount = g.Count(s => s.Next.Criticality == Criticality.Alert)
                })
                .ToListAsync();
            else
                statusCounts = await query
                .GroupBy(s => new { s.Next.Time.Date })
                .Select(g => new PackageStatusCountDto
                {
                    Date = g.Key.Date,
                    HealthyCount = g.Count(s => s.Next.Criticality == Criticality.Healthy),
                    WarningCount = g.Count(s => s.Next.Criticality == Criticality.Warning),
                    AlertCount = g.Count(s => s.Next.Criticality == Criticality.Alert)
                })
                .ToListAsync();

            // Left join to include dates with no statuses
            return
            [
                .. timeBuckets
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
