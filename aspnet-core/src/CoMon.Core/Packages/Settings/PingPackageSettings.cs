using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoMon.Packages.Settings
{
    [Table("CoMonPingPackageSettings")]
    public class PingPackageSettings : Entity<long>
    {
        public const int MaxHostLength = 255;
        public const int MinCycleSeconds = 30;

        [Required]
        [StringLength(MaxHostLength, MinimumLength = 1)]
        public string Host { get; set; }
        [Required]
        [Range(MinCycleSeconds, int.MaxValue)]
        public int CycleSeconds { get; set; }
    }
}