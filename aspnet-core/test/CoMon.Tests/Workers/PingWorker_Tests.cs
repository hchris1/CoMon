using Abp.Domain.Uow;
using CoMon.Packages.Workers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CoMon.Tests.Workers
{
    [Collection("Sequential")]
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
            UsingDbContext(context => DbPreparator.CreateAssetWithPingPackage(context, hasStatus: false));

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
            UsingDbContext(context => DbPreparator.CreateAssetWithPingPackage(context, hasStatus: true));

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
