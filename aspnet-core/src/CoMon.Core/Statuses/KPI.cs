using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoMon.Statuses
{
    [Table("CoMonKPIs")]
    public class KPI : Entity<long>
    {
        public string Name { get; set; }
        public double? Value { get; set; }
        public string Unit { get; set; }

        [ForeignKey(nameof(Status))]
        public long StatusId { get; set; }
        public Status Status { get; set; }
    }
}
