using CoMon.Statuses;
using System;
using System.Collections.Generic;

namespace CoMon.Packages.Dtos
{
    public class PackageStatisticDto
    {
        public PackagePreviewDto Package { get; set; }
        public TimeSpan HealthyDuration { get; set; }
        public double HealthyPercent { get; set; }
        public TimeSpan WarningDuration { get; set; }
        public double WarningPercent { get; set; }
        public TimeSpan AlertDuration { get; set; }
        public double AlertPercent { get; set; }
        public List<PackageHistoryDto> Timeline { get; set; }

        public PackageStatisticDto(PackagePreviewDto package, Dictionary<Criticality, TimeSpan> durationByCriticality,
            List<PackageHistoryDto> timeline, TimeSpan analyzingDuration)
        {
            Package = package;
            Timeline = timeline;
            HealthyDuration = durationByCriticality.ContainsKey(Criticality.Healthy)
                ? durationByCriticality[Criticality.Healthy]
                : TimeSpan.Zero;
            WarningDuration = durationByCriticality.ContainsKey(Criticality.Warning)
                ? durationByCriticality[Criticality.Warning]
                : TimeSpan.Zero;
            AlertDuration = durationByCriticality.ContainsKey(Criticality.Alert)
                ? durationByCriticality[Criticality.Alert]
                : TimeSpan.Zero;
            HealthyPercent = analyzingDuration == TimeSpan.Zero ? 0 : HealthyDuration.TotalMilliseconds / analyzingDuration.TotalMilliseconds;
            WarningPercent = analyzingDuration == TimeSpan.Zero ? 0 : WarningDuration.TotalMilliseconds / analyzingDuration.TotalMilliseconds;
            AlertPercent = analyzingDuration == TimeSpan.Zero ? 0 : AlertDuration.TotalMilliseconds / analyzingDuration.TotalMilliseconds;
        }
    }
}