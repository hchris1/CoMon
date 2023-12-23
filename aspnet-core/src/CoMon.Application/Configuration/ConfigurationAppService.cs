using System.Threading.Tasks;
using Abp.Authorization;
using CoMon.Configuration.Dto;

namespace CoMon.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : CoMonAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeRetentionDays(ChangeRetentionDaysInput input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.RetentionDays, input.Days.ToString());
        }

        public async Task<int> GetRetentionDays()
        {
            return int.Parse(await SettingManager.GetSettingValueAsync(AppSettingNames.RetentionDays));
        }
    }
}
