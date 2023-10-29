using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoMon.Dashboards
{
    [Table("CoMonDashboardTiles")]
    public class DashboardTile : Entity<long>
    {
        [Range(0, int.MaxValue)]
        public int SortIndex { get; set; }
        public DashboardTileType ItemType { get; set; } 
        public long ItemId { get; set; }

        [ForeignKey(nameof(Dashboard))]
        public long DashboardId { get; set; }
        public Dashboard Dashboard { get; set; }
    }

    public enum DashboardTileType
    {
        Group = 0,
        Asset = 1,
        Package = 2
    }
}
