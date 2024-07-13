using CoMon.Assets.Dtos;
using CoMon.Groups.Dtos;
using CoMon.Packages.Dtos;
using System.Collections.Generic;

namespace CoMon.Dashboards.Dtos
{
    public class DashboardTileOptionDto
    {
        public List<GroupPreviewDto> Groups { get; set; }
        public List<AssetPreviewDto> Assets { get; set; }
        public List<PackagePreviewDto> Packages { get; set; }
    }
}