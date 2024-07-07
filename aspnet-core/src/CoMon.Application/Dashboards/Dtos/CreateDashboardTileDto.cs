using Abp.AutoMapper;

namespace CoMon.Dashboards.Dtos
{
    [AutoMapTo(typeof(DashboardTile))]
    public class CreateDashboardTileDto
    {
        public DashboardTileType ItemType { get; set; }
        public long ItemId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Content { get; set; }
    }
}