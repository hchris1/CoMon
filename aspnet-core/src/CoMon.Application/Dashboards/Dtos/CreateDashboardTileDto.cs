using Abp.AutoMapper;

namespace CoMon.Dashboards.Dtos
{
    [AutoMapTo(typeof(DashboardTile))]
    public class CreateDashboardTileDto
    {
        public DashboardTileType ItemType { get; set; }
        public long ItemId { get; set; }
    }
}