using Abp.AutoMapper;
using CoMon.Packages.Settings;

namespace CoMon.Packages.Dtos
{
    [AutoMap(typeof(PingPackageSettings))]
    public class PingPackageSettingsDto
    {
        public string Host { get; set; }
        public int CycleSeconds { get; set; }
    }
}