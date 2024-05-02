using Abp.AutoMapper;
using CoMon.Packages.Dtos;
using System;
using System.Collections.Generic;

namespace CoMon.Statuses.Dtos
{
    [AutoMapFrom(typeof(Status))]
    public class StatusPreviewDto
    {
        public long Id { get; set; }
        public DateTime Time { get; set; }
        public Criticality Criticality { get; set; }
        public TriggerCause TriggerCause { get; set; }
        public bool IsLatest { get; set; }
        public List<string> Messages { get; set; }
        public PackagePreviewDto Package { get; set; }
    }
}
