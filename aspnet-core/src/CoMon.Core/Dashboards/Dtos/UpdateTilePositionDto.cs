namespace CoMon.Dashboards.Dtos
{
    public class UpdateTilePositionDto
    {
        public long TileId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}