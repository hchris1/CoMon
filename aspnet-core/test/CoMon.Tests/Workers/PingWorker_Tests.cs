using Abp.Domain.Uow;
using CoMon.Packages.Workers;
using CoMon.Statuses;
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
        public async Task PerformCheck_AvailableHost_ShouldReturnHealthyStatus()
        {
            // Arrange
            var arrangedAsset = EntityFactory
                .CreateAsset()
                .AddPingPackage(host: "localhost");
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            var status = await _pingWorker.PerformCheck(arrangedAsset.Packages.First());

            // Assert
            Assert.NotNull(status);
            Assert.Equal(Criticality.Healthy, status.Criticality);
        }

        [Fact]
        public async Task PerformCheck_UnavailableHost_ShouldReturnAlertStatus()
        {
            // Arrange
            var arrangedAsset = EntityFactory
                .CreateAsset()
                .AddPingPackage(host: "unavailableHost");
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            var status = await _pingWorker.PerformCheck(arrangedAsset.Packages.First());

            // Assert
            Assert.NotNull(status);
            Assert.Equal(Criticality.Alert, status.Criticality);
        }

        [Fact]
        public void ShouldPerformCheck_NoLastStatus_ShouldReturnTrue()
        {
            // Arrange
            var arrangedAsset = EntityFactory
                .CreateAsset()
                .AddPingPackage(host: "localhost");

            // Act
            var shouldPerformCheck = _pingWorker.ShouldPerformCheck(arrangedAsset.Packages.First());

            // Assert
            Assert.True(shouldPerformCheck);
        }

        [Fact]
        public void ShouldPerformCheck_RecentLastStatus_ShouldReturnFalse()
        {
            // Arrange
            var arrangedAsset = EntityFactory
                .CreateAsset()
                .AddPingPackageWithStatus(Criticality.Healthy);

            // Act
            var shouldPerformCheck = _pingWorker.ShouldPerformCheck(arrangedAsset.Packages.First());

            // Assert
            Assert.False(shouldPerformCheck);
        }

        [Fact]
        public void ShouldPerformCheck_LongGoneLastStatus_ShouldReturnTrue()
        {
            // Arrange
            var arrangedAsset = EntityFactory
                .CreateAsset()
                .AddPingPackageWithStatus(Criticality.Healthy, secondsSinceStatus: 4269);

            // Act
            var shouldPerformCheck = _pingWorker.ShouldPerformCheck(arrangedAsset.Packages.First());

            // Assert
            Assert.True(shouldPerformCheck);
        }

        [Fact]
        public void ShouldPerformCheck_PackageSettingsNull_ShouldReturnFalse()
        {
            // Arrange
            var arrangedAsset = EntityFactory
                .CreateAsset()
                .AddPingPackageWithStatus(Criticality.Healthy);
            arrangedAsset.Packages.First().PingPackageSettings = null;

            // Act
            var shouldPerformCheck = _pingWorker.ShouldPerformCheck(arrangedAsset.Packages.First());

            // Assert
            Assert.False(shouldPerformCheck);
        }

        [Fact]
        public void ShouldPerformCheck_PackageInManualQueue_ShouldReturnTrue()
        {
            // Arrange
            var arrangedAsset = EntityFactory
                .CreateAsset()
                .AddPingPackageWithStatus(Criticality.Healthy);
            _pingWorker.EnqueueManualCheck(arrangedAsset.Packages.First().Id);

            // Act
            var shouldPerformCheck = _pingWorker.ShouldPerformCheck(arrangedAsset.Packages.First());

            // Assert
            Assert.True(shouldPerformCheck);
        }
    }
}
