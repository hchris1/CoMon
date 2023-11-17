using Abp.Domain.Entities;
using CoMon.Packages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoMon.Statuses
{
    [Table("CoMonStatuses")]
    public class Status : Entity<long>
    {
        public DateTime Time { get; set; }
        public Criticality? Criticality { get; set; }
        public List<string> Messages { get; set; } = new();
        public List<KPI> KPIs { get; set; } = new();
        public List<Chart> Charts { get; set; } = new();

        [ForeignKey(nameof(Package))]
        public long PackageId { get; set; }
        public Package Package { get; set; }

        [NotMapped]
        public bool IsLatest { get; set; } = true;
    }

    public enum Criticality
    {
        Healthy = 1,
        Warning = 3,
        Alert = 5
    }
}
