using Abp.Authorization;
using CoMon.Configuration.Dto;
using System.Threading.Tasks;

namespace CoMon.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : CoMonAppServiceBase
    {
        public async Task ChangeRetentionDays(ChangeRetentionDaysInput input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.RetentionDays, input.Days.ToString());
        }

        public async Task<int> GetRetentionDays()
        {
            return int.Parse(await SettingManager.GetSettingValueAsync(AppSettingNames.RetentionDays));
        }

        public async Task ChangeOpenAiKey(ChangeOpenAiKeyInput input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.OpenAiKey, input.OpenAiKey);
        }

        public async Task<string> GetOpenAiKey()
        {
            return await SettingManager.GetSettingValueAsync(AppSettingNames.OpenAiKey);
        }
    }
}
