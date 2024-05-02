using CoMon.Statuses;
using System;
using System.Collections.Generic;

namespace CoMon.Notifications
{
    public class StatusUpdateDto
    {
        public long Id { get; set; }
        public DateTime Time { get; set; }
        public Criticality? PreviousCriticality { get; set; }
        public Criticality? Criticality { get; set; }
        public TriggerCause TriggerCause { get; set; }
        public long PackageId { get; set; }
        public string PackageName { get; set; }
        public long AssetId { get; set; }
        public string AssetName { get; set; }
        public List<long> GroupIds { get; set; }
    }
}