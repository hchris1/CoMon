using CoMon.Statuses;
using System;

namespace CoMon.Packages.Dtos
{
    public class PackageHistoryDto
    {
        public Criticality? Criticality { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public double Percentage { get; set; }
    }
}