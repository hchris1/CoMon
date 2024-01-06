using Abp.Dependency;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using CoMon.EntityFrameworkCore.Seed;
using Microsoft.EntityFrameworkCore;

namespace CoMon.EntityFrameworkCore
{
    [DependsOn(
        typeof(CoMonCoreModule),
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class CoMonEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false;

            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<CoMonDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        CoMonDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        CoMonDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CoMonEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            using (var scope = IocManager.CreateScope())
            {
                var dbContextResolver = scope.Resolve<IDbContextResolver>();
                var context = dbContextResolver.Resolve<CoMonDbContext>(Configuration.DefaultNameOrConnectionString, null);

                Logger.Info("Applying migrations...");
                context.Database.Migrate();
                Logger.Info("Finished migrations");
            }


            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
