using Abp.AutoMapper;

namespace CoMon.Packages.Dtos
{
    [AutoMapTo(typeof(Package))]
    public class UpdatePackageDto : CreatePackageDto
    {
        public long Id { get; set; }
    }
}