using Abp.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoMon.Dashboards
{
    [Table("CoMonDashboards")]
    public class Dashboard : Entity<long>
    {
        public const int MaxNameLength = 256;

        [StringLength(MaxNameLength)]
        public string Name { get; set; }
        public List<DashboardTile> Tiles { get; set; } = [];
    }
}
