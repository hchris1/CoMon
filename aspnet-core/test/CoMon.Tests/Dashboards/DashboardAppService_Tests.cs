using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Runtime.Validation;
using CoMon.Dashboards;
using CoMon.Dashboards.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoMon.Tests.Dashboards
{
    public class DashboardAppService_Tests : CoMonTestBase
    {
        private readonly DashboardAppService _dashboardAppService;
        public DashboardAppService_Tests()
        {
            _dashboardAppService = Resolve<DashboardAppService>();
        }

        [Fact]
        public async Task Get_DashboardExists_ShouldReturnDashboard()
        {
            // Arrange
            var arrangedDashboard = EntityFactory.CreateDashboard();
            UsingDbContext(context => context.Dashboards.Add(arrangedDashboard));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var dashboard = await _dashboardAppService.Get(1);

            // Assert
            Assert.NotNull(dashboard);
        }

        [Fact]
        public async Task Get_DashboardDoesNotExist_ShouldThrowEntityNotFoundException()
        {
            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _dashboardAppService.Get(1));
        }

        [Fact]
        public async Task GetAllPreviews_SingleDashboard_ShouldReturnSingleDashboardPreview()
        {
            // Arrange
            var arrangedDashboard = EntityFactory.CreateDashboard();
            UsingDbContext(context => context.Dashboards.Add(arrangedDashboard));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var dashboardPreviews = await _dashboardAppService.GetAllPreviews();

            // Assert
            Assert.Single(dashboardPreviews);
            Assert.Equal(0, dashboardPreviews[0].GroupCount);
            Assert.Equal(0, dashboardPreviews[0].AssetCount);
            Assert.Equal(0, dashboardPreviews[0].PackageCount);
        }

        [Fact]
        public async Task Create_NameWithWhitespace_ShouldThrowUserFriendlyException()
        {
            // Arrange
            var name = "    Test   ";

            // Act
            await _dashboardAppService.Create(name);

            // Assert
            UsingDbContext(context =>
            {
                var dashboard = context.Dashboards.FirstOrDefault();
                Assert.NotNull(dashboard);
                Assert.Equal("Test", dashboard.Name);
            });
        }

        [Fact]
        public async Task Create_NameWithWhitespaceOnly_ShouldThrowUserFriendlyException()
        {
            // Arrange
            var name = "    ";

            // Act & Assert
            await Assert.ThrowsAsync<AbpValidationException>(() => _dashboardAppService.Create(name));
        }

        [Fact]
        public async Task Delete_DashboardExists_ShouldDeleteDashboard()
        {
            // Arrange
            var arrangedDashboard = EntityFactory.CreateDashboard();
            UsingDbContext(context => context.Dashboards.Add(arrangedDashboard));

            // Act
            await _dashboardAppService.Delete(1);

            // Assert
            UsingDbContext(context =>
            {
                var dashboard = context.Dashboards.FirstOrDefault();
                Assert.Null(dashboard);
            });
        }

        [Fact]
        public async Task UpdateName_DashboardExists_ShouldUpdateName()
        {
            // Arrange
            var arrangedDashboard = EntityFactory.CreateDashboard();
            UsingDbContext(context => context.Dashboards.Add(arrangedDashboard));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await _dashboardAppService.UpdateName(1, "New Name");
            await uow.CompleteAsync();

            // Assert
            UsingDbContext(context =>
            {
                var dashboard = context.Dashboards.FirstOrDefault();
                Assert.NotNull(dashboard);
                Assert.Equal("New Name", dashboard.Name);
            });
        }

        [Fact]
        public async Task UpdateName_DashboardDoesNotExist_ShouldThrowEntityNotFoundException()
        {
            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _dashboardAppService.UpdateName(1, "New Name"));
        }

        [Fact]
        public async Task UpdateName_NullName_ShouldThrowValidationException()
        {
            // Arrange
            var arrangedDashboard = EntityFactory.CreateDashboard();
            UsingDbContext(context => context.Dashboards.Add(arrangedDashboard));

            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<AbpValidationException>(() => _dashboardAppService.UpdateName(1, null));
        }

        [Fact]
        public async Task UpdateName_WhitespaceOnlyName_ShouldThrowValidationException()
        {
            // Arrange
            var arrangedDashboard = EntityFactory.CreateDashboard();
            UsingDbContext(context => context.Dashboards.Add(arrangedDashboard));

            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<AbpValidationException>(() => _dashboardAppService.UpdateName(1, "   "));
        }

        [Fact]
        public async Task AddTile_AddGroupTile_ShouldAddTile()
        {
            // Arrange
            var arrangedDashboard = EntityFactory.CreateDashboard();
            var arrangedGroup = EntityFactory.CreateGroup();
            UsingDbContext(context =>
            {
                context.Dashboards.Add(arrangedDashboard);
                context.Groups.Add(arrangedGroup);
            });

            var tileDto = new CreateDashboardTileDto
            {
                ItemType = DashboardTileType.Group,
                ItemId = 1
            };

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await _dashboardAppService.AddTile(1, tileDto);
            await uow.CompleteAsync();

            // Assert
            UsingDbContext(context =>
            {
                var dashboard = context.Dashboards.FirstOrDefault();
                var tile = context.DashboardTiles.FirstOrDefault();
                Assert.NotNull(dashboard);
                Assert.NotNull(tile);
                Assert.Equal(1, tile.ItemId);
                Assert.Equal(DashboardTileType.Group, tile.ItemType);
            });
        }

        [Fact]
        public async Task AddTile_AddAssetTile_ShouldAddTile()
        {
            // Arrange
            var arrangedDashboard = EntityFactory.CreateDashboard();
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context =>
            {
                context.Dashboards.Add(arrangedDashboard);
                context.Assets.Add(arrangedAsset);
            });

            var tileDto = new CreateDashboardTileDto
            {
                ItemType = DashboardTileType.Asset,
                ItemId = 1
            };

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await _dashboardAppService.AddTile(1, tileDto);
            await uow.CompleteAsync();

            // Assert
            UsingDbContext(context =>
            {
                var dashboard = context.Dashboards.FirstOrDefault();
                var tile = context.DashboardTiles.FirstOrDefault();
                Assert.NotNull(dashboard);
                Assert.NotNull(tile);
                Assert.Equal(1, tile.ItemId);
                Assert.Equal(DashboardTileType.Asset, tile.ItemType);
            });
        }

        [Fact]
        public async Task AddTile_AddPackageTile_ShouldAddTile()
        {
            // Arrange
            var arrangedDashboard = EntityFactory.CreateDashboard();
            var arrangedAsset = EntityFactory.CreateAsset().AddPingPackage();
            UsingDbContext(context =>
            {
                context.Dashboards.Add(arrangedDashboard);
                context.Assets.Add(arrangedAsset);
            });

            var tileDto = new CreateDashboardTileDto
            {
                ItemType = DashboardTileType.Package,
                ItemId = 1
            };

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await _dashboardAppService.AddTile(1, tileDto);
            await uow.CompleteAsync();

            // Assert
            UsingDbContext(context =>
            {
                var dashboard = context.Dashboards.FirstOrDefault();
                var tile = context.DashboardTiles.FirstOrDefault();
                Assert.NotNull(dashboard);
                Assert.NotNull(tile);
                Assert.Equal(1, tile.ItemId);
                Assert.Equal(DashboardTileType.Package, tile.ItemType);
            });
        }

        [Fact]
        public async Task AddTile_DashboardDoesNotExist_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var tileDto = new CreateDashboardTileDto
            {
                ItemType = DashboardTileType.Group,
                ItemId = 1
            };

            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _dashboardAppService.AddTile(1, tileDto));
        }

        [Fact]
        public async Task AddTile_InvalidItemType_ShouldThrowValidationException()
        {
            // Arrange
            var arrangedDashboard = EntityFactory.CreateDashboard();
            UsingDbContext(context => context.Dashboards.Add(arrangedDashboard));

            var tileDto = new CreateDashboardTileDto
            {
                ItemType = (DashboardTileType)int.MaxValue,
                ItemId = 1
            };

            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<AbpValidationException>(() => _dashboardAppService.AddTile(1, tileDto));
        }

        [Fact]
        public async Task DeleteTile_DashboardExistsAndTileExists_ShouldDeleteTile()
        {
            // Arrange
            var arrangedTile = EntityFactory.CreateTile(DashboardTileType.Group, 1);
            var arrangedDashboard = EntityFactory.CreateDashboard().AddTile(arrangedTile);
            UsingDbContext(context =>
            {
                context.Dashboards.Add(arrangedDashboard);
            });

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await _dashboardAppService.DeleteTile(1, 1);
            await uow.CompleteAsync();

            // Assert
            UsingDbContext(context =>
            {
                var dashboard = context.Dashboards.FirstOrDefault();
                var tile = context.DashboardTiles.FirstOrDefault();
                Assert.NotNull(dashboard);
                Assert.Null(tile);
            });
        }

        [Fact]
        public async Task DeleteTile_DashboardDoesNotExist_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var arrangedTile = EntityFactory.CreateTile(DashboardTileType.Group, 1);
            UsingDbContext(context =>
            {
                context.DashboardTiles.Add(arrangedTile);
            });

            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _dashboardAppService.DeleteTile(1, 1));
        }

        [Fact]
        public async Task DeleteTile_TileDoesNotExist_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var arrangedDashboard = EntityFactory.CreateDashboard();
            UsingDbContext(context =>
            {
                context.Dashboards.Add(arrangedDashboard);
            });

            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _dashboardAppService.DeleteTile(1, 1));
        }

        [Fact]
        public async Task GetDashboardTileOptions_ShouldReturnDashboardTileOptions()
        {
            // Arrange
            var arrangedGroup = EntityFactory.CreateGroup();
            var arrangedAsset = EntityFactory.CreateAsset().AddPingPackage();
            UsingDbContext(context =>
            {
                context.Groups.Add(arrangedGroup);
                context.Assets.Add(arrangedAsset);
            });

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var options = await _dashboardAppService.GetDashboardTileOptions();
            await uow.CompleteAsync();

            // Assert
            Assert.NotNull(options);
            Assert.NotEmpty(options.Groups);
            Assert.NotEmpty(options.Assets);
            Assert.NotEmpty(options.Packages);
        }

        //[Fact]
        //public async Task MoveTileUp_DashboardExistsAndTileExists_ShouldMoveTileUp()
        //{
        //    // Arrange
        //    var arrangedTile1 = EntityFactory.CreateTile(DashboardTileType.Group, 1, 0);
        //    var arrangedTile2 = EntityFactory.CreateTile(DashboardTileType.Group, 2, 1);
        //    var arrangedDashboard = EntityFactory.CreateDashboard().AddTile(arrangedTile1).AddTile(arrangedTile2);
        //    UsingDbContext(context =>
        //    {
        //        context.Dashboards.Add(arrangedDashboard);
        //    });

        //    // Act
        //    using var uow = Resolve<IUnitOfWorkManager>().Begin();
        //    await _dashboardAppService.MoveTileUp(1, 2);
        //    await uow.CompleteAsync();

        //    // Assert
        //    UsingDbContext(context =>
        //    {
        //        var dashboard = context.Dashboards.FirstOrDefault();
        //        var tile1 = context.DashboardTiles.FirstOrDefault(t => t.Id == 1);
        //        var tile2 = context.DashboardTiles.FirstOrDefault(t => t.Id == 2);
        //        Assert.NotNull(dashboard);
        //        Assert.NotNull(tile1);
        //        Assert.NotNull(tile2);
        //        Assert.Equal(1, tile1.SortIndex);
        //        Assert.Equal(0, tile2.SortIndex);
        //    });
        //}

        //[Fact]
        //public async Task MoveTileUp_DashboardDoesNotExist_ShouldThrowEntityNotFoundException()
        //{
        //    // Act & Assert
        //    using var uow = Resolve<IUnitOfWorkManager>().Begin();
        //    await Assert.ThrowsAsync<EntityNotFoundException>(() => _dashboardAppService.MoveTileUp(1, 1));
        //}

        //[Fact]
        //public async Task MoveTileUp_TileDoesNotExist_ShouldThrowEntityNotFoundException()
        //{
        //    // Arrange
        //    var arrangedDashboard = EntityFactory.CreateDashboard();
        //    UsingDbContext(context =>
        //    {
        //        context.Dashboards.Add(arrangedDashboard);
        //    });

        //    // Act & Assert
        //    using var uow = Resolve<IUnitOfWorkManager>().Begin();
        //    await Assert.ThrowsAsync<EntityNotFoundException>(() => _dashboardAppService.MoveTileUp(1, 1));
        //}

        //[Fact]
        //public async Task MoveTileUp_TileIsFirst_ShouldThrowValidationException()
        //{
        //    // Arrange
        //    var arrangedTile = EntityFactory.CreateTile(DashboardTileType.Group, 1, 0);
        //    var arrangedDashboard = EntityFactory.CreateDashboard().AddTile(arrangedTile);
        //    UsingDbContext(context =>
        //    {
        //        context.Dashboards.Add(arrangedDashboard);
        //    });

        //    // Act & Assert
        //    using var uow = Resolve<IUnitOfWorkManager>().Begin();
        //    await Assert.ThrowsAsync<AbpValidationException>(() => _dashboardAppService.MoveTileUp(1, 1));
        //}

        //[Fact]
        //public async Task MoveTileDown_DashboardExistsAndTileExists_ShouldMoveTileDown()
        //{
        //    // Arrange
        //    var arrangedTile1 = EntityFactory.CreateTile(DashboardTileType.Group, 1, 0);
        //    var arrangedTile2 = EntityFactory.CreateTile(DashboardTileType.Group, 2, 1);
        //    var arrangedDashboard = EntityFactory.CreateDashboard().AddTile(arrangedTile1).AddTile(arrangedTile2);
        //    UsingDbContext(context =>
        //    {
        //        context.Dashboards.Add(arrangedDashboard);
        //    });

        //    // Act
        //    using var uow = Resolve<IUnitOfWorkManager>().Begin();
        //    await _dashboardAppService.MoveTileDown(1, 1);
        //    await uow.CompleteAsync();

        //    // Assert
        //    UsingDbContext(context =>
        //    {
        //        var dashboard = context.Dashboards.FirstOrDefault();
        //        var tile1 = context.DashboardTiles.FirstOrDefault(t => t.Id == 1);
        //        var tile2 = context.DashboardTiles.FirstOrDefault(t => t.Id == 2);
        //        Assert.NotNull(dashboard);
        //        Assert.NotNull(tile1);
        //        Assert.NotNull(tile2);
        //        Assert.Equal(1, tile1.SortIndex);
        //        Assert.Equal(0, tile2.SortIndex);
        //    });
        //}

        //[Fact]
        //public async Task MoveTileDown_DashboardDoesNotExist_ShouldThrowEntityNotFoundException()
        //{
        //    // Act & Assert
        //    using var uow = Resolve<IUnitOfWorkManager>().Begin();
        //    await Assert.ThrowsAsync<EntityNotFoundException>(() => _dashboardAppService.MoveTileDown(1, 1));
        //}

        //[Fact]
        //public async Task MoveTileDown_TileDoesNotExist_ShouldThrowEntityNotFoundException()
        //{
        //    // Arrange
        //    var arrangedDashboard = EntityFactory.CreateDashboard();
        //    UsingDbContext(context =>
        //    {
        //        context.Dashboards.Add(arrangedDashboard);
        //    });

        //    // Act & Assert
        //    using var uow = Resolve<IUnitOfWorkManager>().Begin();
        //    await Assert.ThrowsAsync<EntityNotFoundException>(() => _dashboardAppService.MoveTileDown(1, 1));
        //}

        //[Fact]
        //public async Task MoveTileDown_TileIsLast_ShouldThrowValidationException()
        //{
        //    // Arrange
        //    var arrangedTile = EntityFactory.CreateTile(DashboardTileType.Group, 1, 0);
        //    var arrangedDashboard = EntityFactory.CreateDashboard().AddTile(arrangedTile);
        //    UsingDbContext(context =>
        //    {
        //        context.Dashboards.Add(arrangedDashboard);
        //    });

        //    // Act & Assert
        //    using var uow = Resolve<IUnitOfWorkManager>().Begin();
        //    await Assert.ThrowsAsync<AbpValidationException>(() => _dashboardAppService.MoveTileDown(1, 1));
        //}
    }
}
