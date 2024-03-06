using Abp.Domain.Uow;
using CoMon.Statuses;
using System.Threading.Tasks;
using Xunit;

namespace CoMon.Tests.Statuses
{
    public class StatusAppService_Tests : CoMonTestBase
    {
        private readonly StatusAppService _statusAppService;

        public StatusAppService_Tests()
        {
            _statusAppService = Resolve<StatusAppService>();
        }

        [Fact]
        public async Task Get_StatusIsLatest_ReturnsStatusWithIsLatest()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset().AddPingPackageWithStatus(Criticality.Healthy);
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var status = await _statusAppService.Get(1);

            // Assert
            Assert.NotNull(status);
            Assert.Equal(Criticality.Healthy, status.Criticality);
            Assert.True(status.IsLatest);
        }
    }
}
