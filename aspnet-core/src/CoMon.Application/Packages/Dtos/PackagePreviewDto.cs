using Abp.AutoMapper;
using CoMon.Assets.Dtos;
using System;

namespace CoMon.Packages.Dtos
{
    [AutoMapFrom(typeof(Package))]
    public class PackagePreviewDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public PackageType Type { get; set; }
        public AssetPreviewDto Asset { get; set; }
    }
}
