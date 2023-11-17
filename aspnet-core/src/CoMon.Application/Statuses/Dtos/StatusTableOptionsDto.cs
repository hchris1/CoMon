using CoMon.Assets.Dtos;
using CoMon.Groups.Dtos;
using System.Collections.Generic;

namespace CoMon.Statuses.Dtos
{
    public class StatusTableOptionsDto
    {
        public List<AssetPreviewDto> Assets { get; set; }
        public List<GroupPreviewDto> Groups { get; set; }
    }
}
