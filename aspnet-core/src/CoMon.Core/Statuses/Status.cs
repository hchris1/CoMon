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
        public TriggerCause TriggerCause { get; set; } = TriggerCause.Unknown;
        public List<string> Messages { get; set; } = [];
        public List<KPI> KPIs { get; set; } = [];
        public List<Chart> Charts { get; set; } = [];

        [ForeignKey(nameof(Package))]
        public long PackageId { get; set; }
        public Package Package { get; set; }

        [NotMapped]
        public bool IsLatest { get; set; } = true;
    }

    public enum TriggerCause
    {
        Unknown = 0,
        Initialized = 1,
        Scheduled = 2,
        Manual = 3,
        External = 4,
    }

    public enum Criticality
    {
        Healthy = 1,
        Warning = 3,
        Alert = 5
    }
}
