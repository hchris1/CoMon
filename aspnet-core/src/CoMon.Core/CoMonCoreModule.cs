using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Security;
using Abp.Threading.BackgroundWorkers;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using CoMon.Authorization.Roles;
using CoMon.Authorization.Users;
using CoMon.Configuration;
using CoMon.Localization;
using CoMon.MultiTenancy;
using CoMon.Notifications;
using CoMon.Packages.Workers;
using CoMon.Retention;
using CoMon.Timing;

namespace CoMon
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class CoMonCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            // Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            CoMonLocalizationConfigurer.Configure(Configuration.Localization);

            // Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = CoMonConsts.MultiTenancyEnabled;

            // Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            Configuration.Settings.Providers.Add<AppSettingProvider>();
            
            Configuration.Settings.SettingEncryptionConfiguration.DefaultPassPhrase = CoMonConsts.DefaultPassPhrase;
            SimpleStringCipher.DefaultPassPhrase = CoMonConsts.DefaultPassPhrase;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CoMonCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<PingWorker>());
            workManager.Add(IocManager.Resolve<HttpWorker>());
            workManager.Add(IocManager.Resolve<RetentionWorker>());
            IocManager.Resolve<HaSender>();
        } 
    }
}
