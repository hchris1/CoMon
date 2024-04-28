using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoMon.Packages.Settings
{
    [Table("CoMonRtspPackageSettings")]
    public class RtspPackageSettings : Entity<long>, IPackageSettings
    {
        public const int MinCycleSeconds = 30;
        public const int MaxUrlLength = 255;

        [Required]
        [StringLength(MaxUrlLength, MinimumLength = 1)]
        public string Url { get; set; }

        [Required]
        [Range(MinCycleSeconds, int.MaxValue)]
        public int CycleSeconds { get; set; }

        [Required]
        public RtspPackageMethod Method { get; set; }
    }

    public enum RtspPackageMethod
    {
        Describe = 0,
        Options = 1,
        Play = 2,
        Pause = 3,
        Teardown = 4,
        Setup = 5
    }
}