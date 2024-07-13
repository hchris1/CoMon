using Abp.AutoMapper;

namespace CoMon.Dashboards.Dtos
{
    [AutoMapFrom(typeof(DashboardTile))]
    public class DashboardTileDto
    {
        public long Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Content { get; set; }
        public DashboardTileType ItemType { get; set; }
        public long ItemId { get; set; }
    }
}