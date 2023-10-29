using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CoMon.EntityFrameworkCore;
using CoMon.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace CoMon.Web.Tests
{
    [DependsOn(
        typeof(CoMonWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class CoMonWebTestModule : AbpModule
    {
        public CoMonWebTestModule(CoMonEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CoMonWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(CoMonWebMvcModule).Assembly);
        }
    }
}