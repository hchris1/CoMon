using Abp.AutoMapper;

namespace CoMon.Dashboards.Dtos
{
    [AutoMapFrom(typeof(Dashboard))]
    public class DashboardPreviewDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int GroupCount { get; set; }
        public int AssetCount { get; set; }
        public int PackageCount { get; set; }
    }
}