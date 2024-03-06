using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using CoMon.Statuses;
using System.Linq;
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
            Assert.Equal(arrangedAsset.Packages[0].Statuses[0].KPIs[0].Value, status.KPIs[0].ThirtyDayMax);
            Assert.Equal(arrangedAsset.Packages[0].Statuses[0].KPIs[0].Value, status.KPIs[0].ThirtyDayMin);
            Assert.Equal(arrangedAsset.Packages[0].Statuses[0].KPIs[0].Value, status.KPIs[0].ThirtyDayAverage);
        }

        [Fact]
        public async Task Get_StatusDoesNotExist_ThrowsException()
        {
            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _statusAppService.Get(1));
        }

        [Fact]
        public async Task GetPreview_StatusIsLatest_ReturnsStatusWithIsLatest()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset().AddPingPackageWithStatus(Criticality.Healthy);
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var status = await _statusAppService.GetPreview(1);

            // Assert
            Assert.NotNull(status);
            Assert.Equal(Criticality.Healthy, status.Criticality);
            Assert.True(status.IsLatest);
        }

        [Fact]
        public async Task GetPreview_StatusDoesNotExist_ThrowsException()
        {
            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _statusAppService.GetPreview(1));
        }

        [Fact]
        public async Task GetHistory_SingleStatus_ReturnsStatusOnly()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset().AddPingPackageWithStatus(Criticality.Healthy);
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var history = await _statusAppService.GetHistory(1);

            // Assert
            Assert.Equal(1, history.Status.Id);
            Assert.Null(history.PreviousStatus);
            Assert.Null(history.NextStatus);
            Assert.Null(history.LatestStatus);
        }

        [Fact]
        public async Task GetHistory_StatusDoesNotExist_ThrowsException()
        {
            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _statusAppService.GetHistory(1));
        }

        [Fact]
        public async Task GetStatusTableOptions_AssetsAndGroupsExist_ReturnsOptions()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset().AddPingPackageWithStatus(Criticality.Healthy);
            var arrangedGroup = EntityFactory.CreateGroup();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));
            UsingDbContext(context => context.Groups.Add(arrangedGroup));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var options = await _statusAppService.GetStatusTableOptions();

            // Assert
            Assert.NotNull(options);
            Assert.NotEmpty(options.Assets);
            Assert.NotEmpty(options.Groups);
        }

        [Fact]
        public async Task GetStatusTable_AssetsAndGroupsExist_ReturnsTable()
        {
            // Arrange
            var arrangedGroup = EntityFactory.CreateGroup().AddAsset(EntityFactory.CreateAsset().AddPingPackageWithStatus(Criticality.Healthy));
            UsingDbContext(context => context.Groups.Add(arrangedGroup));
            var request = new PagedResultRequestDto()
            {
                MaxResultCount = 10,
                SkipCount = 0
            };

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var table = await _statusAppService.GetStatusTable(request, 1, 1, 1, Criticality.Healthy, true);

            // Assert
            Assert.NotNull(table);
            Assert.Equal(1, table.TotalCount);
            Assert.NotEmpty(table.Items);
        }

        [Fact]
        public async Task GetLatestStatusPreview_StatusAvailable_ReturnsStatus()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset().AddPingPackageWithStatus(Criticality.Healthy);
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var status = await _statusAppService.GetLatestStatusPreview(1);

            // Assert
            Assert.NotNull(status);
            Assert.Equal(Criticality.Healthy, status.Criticality);
            Assert.True(status.IsLatest);
        }

        [Fact]
        public async Task GetLatestStatusPreview_StatusNotAvailable_ReturnsNull()
        {
            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var status = await _statusAppService.GetLatestStatusPreview(1);

            // Assert
            Assert.Null(status);
        }

        [Fact]
        public void DeleteAll_StatusesExist_DeletesStatuses()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset().AddPingPackageWithStatus(Criticality.Healthy);
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            _statusAppService.DeleteAll();
            uow.Complete();

            // Assert
            var statuses = UsingDbContext(context => context.Statuses.ToList());
            Assert.Empty(statuses);
        }
    }
}
