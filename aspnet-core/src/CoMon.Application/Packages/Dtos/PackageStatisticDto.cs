using System;

namespace CoMon.Packages.Dtos
{
    public class PackageStatisticDto
    {
        public long PackageId { get; set; }
        public TimeSpan UnknownDuration { get; set; }
        public double UnknownPercent { get; set; }
        public TimeSpan HealthyDuration { get; set; }
        public double HealthyPercent { get; set; }
        public TimeSpan WarningDuration { get; set; }
        public double WarningPercent { get; set; }
        public TimeSpan AlertDuration { get; set; }
        public double AlertPercent { get; set; }
    }
}