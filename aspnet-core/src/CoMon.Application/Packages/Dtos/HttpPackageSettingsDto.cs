using Abp.AutoMapper;
using CoMon.Packages.Settings;
using System.ComponentModel.DataAnnotations;

namespace CoMon.Packages.Dtos
{
    [AutoMap(typeof(HttpPackageSettings))]
    public class HttpPackageSettingsDto
    {
        [Required]
        [StringLength(HttpPackageSettings.MaxUrlLength, MinimumLength = 1)]
        public string Url { get; set; }
        [Required]
        [Range(HttpPackageSettings.MinCycleSeconds, int.MaxValue)]
        public int CycleSeconds { get; set; }
        public HttpPackageMethod Method { get; set; }
        public string Headers { get; set; }
        public string Body { get; set; }
        public HttpPackageBodyEncoding Encoding { get; set; }
        public bool IgnoreSslErrors { get; set; }
    }
}