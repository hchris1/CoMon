using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoMon.Packages.Settings
{
    [Table("CoMonHttpPackageSettings")]
    public class HttpPackageSettings : Entity<long>
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
        public HttpPackageMethod Method { get; set; }

        [Required]
        public string Headers { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public HttpPackageBodyEncoding Encoding { get; set; }

        [Required]
        public bool IgnoreSslErrors { get; set; }
    }

    public enum HttpPackageMethod
    {
        Get = 0,
        Post = 1,
        Put = 2,
        Patch = 3,
        Delete = 4,
        Options = 5,
    }

    public enum HttpPackageBodyEncoding
    {
        Json = 0,
        Xml = 1,
    }
}