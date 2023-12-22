using Abp.AutoMapper;

namespace CoMon.Packages.Dtos
{

    [AutoMapTo(typeof(Package))]
    public class CreatePackageDto
    {
        public string Name { get; set; }
        public PackageType Type { get; set; }
        public long AssetId { get; set; }
        public PingPackageSettingsDto PingPackageSettings { get; set; }
        public HttpPackageSettingsDto HttpPackageSettings { get; set; }
    }
}