using Abp.AutoMapper;
using CoMon.Groups.Dtos;
using CoMon.Images.Dtos;
using CoMon.Packages.Dtos;
using System.Collections.Generic;

namespace CoMon.Assets.Dtos
{
    [AutoMapFrom(typeof(Asset))]
    public class AssetDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public List<ImageDto> Images { get; set; }
        public List<PackageDto> Packages { get; set; }
        public GroupPreviewDto Group { get; set; }
    }
}
