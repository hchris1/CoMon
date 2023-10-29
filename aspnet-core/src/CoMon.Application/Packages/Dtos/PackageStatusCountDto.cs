using System;
using System.Collections.Generic;

namespace CoMon.Packages.Dtos
{
    public class PackageStatusCountDto
    {
        public DateTime Date { get; set; }
        public int HealthyCount { get; set; }
        public int WarningCount { get; set; }
        public int AlertCount { get; set; }
    }
}