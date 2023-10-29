using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace CoMon.Packages.Dtos
{
    [AutoMapTo(typeof(Package))]
    public class UpdatePackageDto : CreatePackageDto
    {
        [Required]
        [Range(1, long.MaxValue)]
        public long Id { get; set; }
    }
}