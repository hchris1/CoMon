using Abp.AutoMapper;
using CoMon.Packages.Settings;
using System.ComponentModel.DataAnnotations;

namespace CoMon.Packages.Dtos
{
    [AutoMap(typeof(PingPackageSettings))]
    public class PingPackageSettingsDto
    {
        [Required]
        [StringLength(PingPackageSettings.MaxHostLength, MinimumLength = 1)]
        public string Host { get; set; }
        [Required]
        [Range(PingPackageSettings.MinCycleSeconds, int.MaxValue)]
        public int CycleSeconds { get; set; }
    }
}