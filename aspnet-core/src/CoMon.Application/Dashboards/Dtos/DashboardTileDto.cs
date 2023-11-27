using Abp.AutoMapper;

namespace CoMon.Dashboards.Dtos
{
    [AutoMapFrom(typeof(DashboardTile))]
    public class DashboardTileDto
    {
        public long Id { get; set; }
        public int SortIndex { get; set; }
        public DashboardTileType ItemType { get; set; }
        public long ItemId { get; set; }
    }
}