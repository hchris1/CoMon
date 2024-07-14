using Abp.AutoMapper;
using CoMon.Groups.Dtos;

namespace CoMon.Assets.Dtos
{
    [AutoMapFrom(typeof(Asset))]
    public class AssetPreviewDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public GroupPreviewDto Group { get; set; }
    }
}
