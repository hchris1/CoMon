using Abp.Runtime.Validation;
using CoMon.Packages.Dtos;
using CoMon.Statuses;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CoMon.Packages
{
    public static class PackageAppServiceHelper
    {

        public static void ValidateSettings(CreatePackageDto input)
        {
            switch (input.Type)
            {
                case PackageType.Ping:
                    ValidatePingSettings(input.PingPackageSettings);
                    break;

                case PackageType.Http:
                    ValidateHttpSettings(input.HttpPackageSettings);
                    break;

                case PackageType.External:
                    break;

                default:
                    throw new AbpValidationException("Unknown PackageType.");
            }
        }

        private static void ValidatePingSettings(PingPackageSettingsDto input)
        {
            if (input == null)
                throw new AbpValidationException("PingPackageSettings may not be null.");
        }

        private static void ValidateHttpSettings(HttpPackageSettingsDto input)
        {
            if (input == null)
                throw new AbpValidationException("HttpPackageSettings may not be null.");

            // Validate Body
            try
            {
                switch (input.Encoding)
                {
                    case HttpPackageBodyEncoding.Json:
                        if (!string.IsNullOrWhiteSpace(input.Body))
                            JToken.Parse(input.Body); // Using Newtonsoft.Json.Linq;
                        break;

                    case HttpPackageBodyEncoding.Xml:
                        if (!string.IsNullOrWhiteSpace(input.Body))
                            new XmlDocument().LoadXml(input.Body);
                        break;

                    default:
                        throw new AbpValidationException("Invalid body encoding.");
                }
            }
            catch (Exception ex)
            {
                throw new AbpValidationException("Invalid body content: " + ex.Message);
            }

            // Validate Headers
            try
            {
                if (!string.IsNullOrWhiteSpace(input.Headers))
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(input.Headers);
            }
            catch (Exception ex)
            {
                throw new AbpValidationException("Invalid headers format: " + ex.Message);
            }
        }

        private static List<PackageHistoryDto> MapToPackageHistoryDtos(List<TimeCriticality> entries)
        {
            var result = new List<PackageHistoryDto>();
            for (int i = 1; i < entries.Count; i++)
            {
                result.Add(new PackageHistoryDto()
                {
                    Criticality = entries[i-1].Criticality,
                    From = entries[i - 1].Time,
                    To = entries[i].Time,
                    Percentage = 0
                });
            }
            return result;
        }

        public static List<PackageHistoryDto> MergePackageHistoryDtos(List<PackageHistoryDto> entries)
        {
            for (int i = entries.Count - 1; i > 0; i--)
            {
                if (entries[i].Criticality == entries[i - 1].Criticality)
                {
                    entries[i - 1].To = entries[i].To;
                    entries.RemoveAt(i);
                }
            }
            return entries;
        }

        public static List<TimeCriticality> MergeTimeCriticalities(List<TimeCriticality> entries)
        {
            var result = new List<TimeCriticality>();
            for (int i = 1; i < entries.Count; i++)
            {
                if (entries[i].Criticality != null && entries[i].Criticality != entries[i - 1].Criticality)
                    result.Add(entries[i]);
            }
            return result;
        }

        public static List<PackageHistoryDto> BuildTimeline(List<TimeCriticality> entries, DateTime from, TimeSpan analyzingDuration)
        {
            var packageHistoryItems = MergePackageHistoryDtos(MapToPackageHistoryDtos(entries));

            packageHistoryItems.ForEach(r => r.Percentage = (double)(r.To - r.From).Ticks / (double)analyzingDuration.Ticks);

            var fullPercentage = packageHistoryItems.Sum(r => r.Percentage);

            if (fullPercentage < 1)
                packageHistoryItems.Insert(0, new PackageHistoryDto()
                {
                    From = from,
                    To = packageHistoryItems.FirstOrDefault()?.From ?? from + analyzingDuration,
                    Percentage = 1 - fullPercentage,
                    Criticality = null
                });

            return packageHistoryItems;
        }

        public static DateTime FloorToPreviousHour(DateTime dateTime)
        {
            return new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
        }

        public static List<DateTime> GenerateTimeBuckets(DateTime startDate, DateTime endDate, bool useHourBuckets)
        {
            // TODO: What are these constants being added?
            if (useHourBuckets)
                return Enumerable.Range(0, Convert.ToInt32((endDate - startDate).TotalHours) + 3)
                .Select(offset => FloorToPreviousHour(startDate).AddHours(offset))
                .ToList();

            return Enumerable.Range(0, (endDate - startDate).Days + 2)
                .Select(offset => startDate.AddDays(offset).Date)
                .ToList();
        }

        public static (DateTime startDate, DateTime endDate) GetDateRange(int numOfHours)
        {
            var utcNow = DateTime.UtcNow;
            return (utcNow.AddHours(-numOfHours), FloorToPreviousHour(utcNow));
        }

        public static async Task<List<TimeCriticality>> GetStatusesSinceCutOff(IRepository<Status, long> statusRepository,
            long packageId, DateTime from, DateTime utcNow, bool addVirtualStatusAtUtcNow = true)
        {
            var entries = await statusRepository
                    .GetAll()
                    .Where(s => s.PackageId == packageId && s.Time >= from)
                    .Where(s => s.Criticality != null)
                    .Select(s => new TimeCriticality(s.Time, s.Criticality))
                    .ToListAsync();

            var entryBeforeCutOff = await statusRepository
                .GetAll()
                .Where(s => s.PackageId == packageId && s.Time < from)
                .Where(s => s.Criticality != null)
                .OrderByDescending(s => s.Time)
                .Select(s => new TimeCriticality(from, s.Criticality))
                .FirstOrDefaultAsync();

            if (entryBeforeCutOff != null)
                entries.Add(entryBeforeCutOff);

            entries = [.. entries.OrderBy(s => s.Time)];

            if (entries.Count() != 0 && addVirtualStatusAtUtcNow)
                entries.Add(new TimeCriticality(utcNow, entries.Last().Criticality));

            return entries;
        }

        public static Dictionary<Criticality, TimeSpan> CalculateDurationByCriticality(List<TimeCriticality> entries)
        {
            var durationByCriticality = new Dictionary<Criticality, TimeSpan>()
            {
                [Criticality.Healthy] = TimeSpan.Zero,
                [Criticality.Warning] = TimeSpan.Zero,
                [Criticality.Alert] = TimeSpan.Zero
            };

            for (int i = 1; i < entries.Count; i++)
            {
                if (!entries[i].Criticality.HasValue)
                    continue;

                durationByCriticality[entries[i].Criticality.Value] += entries[i].Time - entries[i - 1].Time;
            }

            return durationByCriticality;
        }

        public static List<PackageStatusCountDto> CreatePackageStatusCountDtos(List<TimeCriticality> entries, bool useHourBuckets)
        {
            List<PackageStatusCountDto> statusCounts;
            if (useHourBuckets)
                statusCounts = entries
                .GroupBy(s => new { s.Time.Date, s.Time.Hour })
                .Select(g => new PackageStatusCountDto
                {
                    Date = g.Key.Date.AddHours(g.Key.Hour),
                    HealthyCount = g.Count(s => s.Criticality == Criticality.Healthy),
                    WarningCount = g.Count(s => s.Criticality == Criticality.Warning),
                    AlertCount = g.Count(s => s.Criticality == Criticality.Alert)
                })
                .ToList();
            else
                statusCounts = entries
                .GroupBy(s => new
                {
                    s.Time.Date
                })
                .Select(g => new PackageStatusCountDto
                {
                    Date = g.Key.Date,
                    HealthyCount = g.Count(s => s.Criticality == Criticality.Healthy),
                    WarningCount = g.Count(s => s.Criticality == Criticality.Warning),
                    AlertCount = g.Count(s => s.Criticality == Criticality.Alert)
                })
                .ToList();

            return statusCounts;
        }

        public static List<PackageStatusCountDto> FillMissingBuckets(List<DateTime> timeBuckets, List<PackageStatusCountDto> statusCounts)
        {
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

        public static async Task<List<PackageStatusCountDto>> GetPackageStatusBuckets(IRepository<Status, long> statusRepository, long packageId, int hours, bool useHourBuckets, bool onlyChanges)
        {
            var (startDate, endDate) = GetDateRange(hours);

            var timeBuckets = GenerateTimeBuckets(startDate, endDate, useHourBuckets);

            var entries = await GetStatusesSinceCutOff(statusRepository, packageId, startDate, DateTime.UtcNow, false);

            if (onlyChanges)
                entries = MergeTimeCriticalities(entries);

            var statusCounts = CreatePackageStatusCountDtos(entries, useHourBuckets);

            return FillMissingBuckets(timeBuckets, statusCounts);
        }
    }

    public record TimeCriticality(DateTime Time, Criticality? Criticality);
}
