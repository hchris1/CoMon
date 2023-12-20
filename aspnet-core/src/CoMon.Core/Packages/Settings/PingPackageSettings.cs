using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoMon.Packages.Settings
{
    [Table("CoMonPingPackageSettings")]
    public class PingPackageSettings : Entity<long>
    {
        [Required]
        public string Host { get; set; }
        [Required]
        [Range(30, int.MaxValue)]
        public int CycleSeconds { get; set; }
    }
}