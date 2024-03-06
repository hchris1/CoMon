using Abp.Domain.Uow;
using CoMon.Packages.Workers;
using CoMon.Statuses;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CoMon.Tests.Workers
{
    public class PingWorker_Tests : CoMonTestBase
    {
        private readonly PingWorker _pingWorker;

        public PingWorker_Tests()
        {
            _pingWorker = Resolve<PingWorker>();
        }

        [Fact]
        public async Task DoWorkAsync_ShouldProcessPackages()
        {
            // Arrange
            var arrangedAsset = EntityFactory
                .CreateAsset()
                .AddPingPackage();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await _pingWorker.PerformChecks();
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var status = context.Statuses.FirstOrDefault();
                Assert.NotNull(status);
            });
        }

        [Fact]
        public async Task DoWorkAsync_ShouldNotProcessPackages()
        {
            // Arrange
            var arrangedAsset = EntityFactory
                .CreateAsset()
                .AddPingPackageWithStatus(Criticality.Healthy);
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await _pingWorker.PerformChecks();
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var statusCount = context.Statuses.Count();
                Assert.Equal(1, statusCount);
            });
        }
    }
}
