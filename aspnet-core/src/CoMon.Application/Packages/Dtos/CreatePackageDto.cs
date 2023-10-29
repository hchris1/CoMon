using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace CoMon.Packages.Dtos
{

    [AutoMapTo(typeof(Package))]
    public class CreatePackageDto
    {
        [Required]
        [StringLength(Package.MaxNameLength, MinimumLength = 1)]
        public string Name { get; set; }
        [Required]
        public PackageType Type { get; set; }
        [Required]
        [Range(1, long.MaxValue)]
        public long AssetId { get; set; }
        public PingPackageSettingsDto PingPackageSettings { get; set; }
        public HttpPackageSettingsDto HttpPackageSettings { get; set; }
    }
}