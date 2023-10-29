using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CoMon.Authorization;

namespace CoMon
{
    [DependsOn(
        typeof(CoMonCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class CoMonApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<CoMonAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(CoMonApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
