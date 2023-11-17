using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using CoMon.Configuration.Dto;

namespace CoMon.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : CoMonAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
