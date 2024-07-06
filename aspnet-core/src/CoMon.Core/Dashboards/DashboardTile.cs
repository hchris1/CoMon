using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoMon.Dashboards
{
    [Table("CoMonDashboardTiles")]
    public class DashboardTile : Entity<long>
    {
        public DashboardTileType ItemType { get; set; }
        public long ItemId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Content { get; set; }

        [ForeignKey(nameof(Dashboard))]
        public long DashboardId { get; set; }
        public Dashboard Dashboard { get; set; }
    }

    public enum DashboardTileType
    {
        Group = 0,
        Asset = 1,
        Package = 2,
        Markdown = 3,
    }
}
