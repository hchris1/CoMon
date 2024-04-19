using Abp.AutoMapper;
using CoMon.Packages.Settings;
using System.ComponentModel.DataAnnotations;

namespace CoMon.Packages.Dtos
{
    [AutoMap(typeof(RtspPackageSettings))]
    public class RtspPackageSettingsDto
    {
        [Required]
        [StringLength(RtspPackageSettings.MaxUrlLength, MinimumLength = 1)]
        public string Url { get; set; }
        [Required]
        [Range(RtspPackageSettings.MinCycleSeconds, int.MaxValue)]
        public int CycleSeconds { get; set; }
        public RtspPackageMethod Method { get; set; }
    }
}