using System.Collections.Generic;
using Abp.Configuration;

namespace CoMon.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.RetentionDays,
                    "30",
                    scopes: SettingScopes.Application
                ),
                new SettingDefinition(AppSettingNames.OpenAiKey,
                    "",
                    scopes: SettingScopes.Application
                                                                     )
            };
        }
    }
}
