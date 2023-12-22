using Abp.AutoMapper;

namespace CoMon.Packages.Dtos
{
    [AutoMap(typeof(HttpPackageSettings))]
    public class HttpPackageSettingsDto
    {
        public string Url { get; set; }
        public int CycleSeconds { get; set; }
        public HttpPackageMethod Method { get; set; }
        public string Headers { get; set; }
        public string Body { get; set; }
        public HttpPackageBodyEncoding Encoding { get; set; }
        public bool IgnoreSslErrors { get; set; }
    }
}