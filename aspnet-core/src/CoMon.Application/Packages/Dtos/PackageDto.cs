using Abp.AutoMapper;
using CoMon.Statuses;
using System;

namespace CoMon.Packages.Dtos
{
    [AutoMapFrom(typeof(Package))]
    public class PackageDto
    {
        public long Id { get; set; }
        public long AssetId { get; set; }
        public string Name { get; set; }
        public PackageType Type { get; set; }
        public Guid Guid { get; set; }
        public PingPackageSettingsDto PingPackageSettings { get; set; }
        public Criticality? LastCriticality { get; set; }
    }
}
