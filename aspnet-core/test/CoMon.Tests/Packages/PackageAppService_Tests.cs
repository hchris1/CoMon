using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Runtime.Validation;
using CoMon.Packages;
using CoMon.Packages.Dtos;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoMon.Tests.Packages
{
    public class PackageAppService_Tests : CoMonTestBase
    {
        private readonly PackageAppService _packageAppService;
        public PackageAppService_Tests()
        {
            _packageAppService = Resolve<PackageAppService>();
        }

        [Fact]
        public async Task Get_PackageExists_ReturnsPackage()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset().AddPingPackageWithStatus(Criticality.Healthy);
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var package = await _packageAppService.Get(1);

            // Assert
            Assert.NotNull(package);
            Assert.Equal(1, package.Id);
            Assert.Equal(1, package.AssetId);
            Assert.NotNull(package.PingPackageSettings.Host);
            Assert.Equal(Criticality.Healthy, package.LastCriticality);
        }

        [Fact]
        public async Task Get_PackageDoesNotExist_ThrowsException()
        {
            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _packageAppService.Get(1));
        }

        [Fact]
        public async Task GetPreview_PackageExists_ReturnsPackagePreview()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset().AddPingPackageWithStatus(Criticality.Healthy);
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var package = await _packageAppService.GetPreview(1);

            // Assert
            Assert.NotNull(package);
            Assert.Equal(1, package.Id);
            Assert.Equal(1, package.Asset.Id);
        }

        [Fact]
        public async Task GetPreview_PackageDoesNotExist_ThrowsException()
        {
            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _packageAppService.GetPreview(1));
        }

        [Fact]
        public async Task Create_AssetDoesNotExist_ThrowsException()
        {
            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _packageAppService.Create(new CreatePackageDto
            {
                AssetId = 1,
                Name = "Test Package",
                Type = PackageType.Ping,
                PingPackageSettings = new PingPackageSettingsDto
                {
                    Host = "localhost",
                    CycleSeconds = 60
                }
            }));
        }

        [Fact]
        public async Task Create_ValidPingPackage_Created()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var id = await _packageAppService.Create(new CreatePackageDto
            {
                AssetId = 1,
                Name = "Test Package",
                Type = PackageType.Ping,
                PingPackageSettings = new PingPackageSettingsDto
                {
                    Host = "localhost",
                    CycleSeconds = 60
                }
            });
            await uow.CompleteAsync();

            // Assert
            using var evalUow = Resolve<IUnitOfWorkManager>().Begin();
            var package = await _packageAppService.Get(id);
            Assert.NotNull(package);
            Assert.Equal(1, package.AssetId);
            Assert.Equal("Test Package", package.Name);
            Assert.Equal(PackageType.Ping, package.Type);
            Assert.NotNull(package.PingPackageSettings);
            Assert.Equal("localhost", package.PingPackageSettings.Host);
        }

        [Fact]
        public async Task Create_PingPackageSettingsAreNull_ThrowsException()
        {
            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<AbpValidationException>(async () => await _packageAppService.Create(new CreatePackageDto
            {
                AssetId = 1,
                Name = "Test Package",
                Type = PackageType.Ping,
                PingPackageSettings = null
            }));
        }

        [Fact]
        public async Task Create_PingPackageCycleTimeTooShort_ThrowsException()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<AbpValidationException>(async () => await _packageAppService.Create(new CreatePackageDto
            {
                AssetId = 1,
                Name = "Test Package",
                Type = PackageType.Ping,
                PingPackageSettings = new PingPackageSettingsDto
                {
                    Host = "localhost",
                    CycleSeconds = 0
                }
            }));
        }

        [Fact]
        public async Task Create_HttpPackageValidJsonBody_Created()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var id = await _packageAppService.Create(new CreatePackageDto
            {
                AssetId = 1,
                Name = "Test Package",
                Type = PackageType.Http,
                HttpPackageSettings = new HttpPackageSettingsDto
                {
                    Url = "http://localhost",
                    CycleSeconds = 60,
                    Encoding = HttpPackageBodyEncoding.Json,
                    Headers = "",
                    Body = "{\"key\": \"value\"}"
                }
            });
            await uow.CompleteAsync();

            // Assert
            using var evalUow = Resolve<IUnitOfWorkManager>().Begin();
            var package = await _packageAppService.Get(id);
            Assert.NotNull(package);
            Assert.Equal(1, package.AssetId);
        }

        [Fact]
        public async Task Create_HttpPackageValidXmlBody_Created()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var id = await _packageAppService.Create(new CreatePackageDto
            {
                AssetId = 1,
                Name = "Test Package",
                Type = PackageType.Http,
                HttpPackageSettings = new HttpPackageSettingsDto
                {
                    Url = "http://localhost",
                    CycleSeconds = 60,
                    Encoding = HttpPackageBodyEncoding.Xml,
                    Headers = "",
                    Body = "<root><key>value</key></root>"
                }
            });
            await uow.CompleteAsync();

            // Assert
            using var evalUow = Resolve<IUnitOfWorkManager>().Begin();
            var package = await _packageAppService.Get(id);
            Assert.NotNull(package);
            Assert.Equal(1, package.AssetId);
        }

        [Fact]
        public async Task Create_HttpPackageCycleTimeTooShort_ThrowsException()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<AbpValidationException>(async () => await _packageAppService.Create(new CreatePackageDto
            {
                AssetId = 1,
                Name = "Test Package",
                Type = PackageType.Http,
                HttpPackageSettings = new HttpPackageSettingsDto
                {
                    Url = "http://localhost",
                    CycleSeconds = 0,
                    Headers = ""
                }
            }));
        }

        [Fact]
        public async Task Create_HttpPackageInvalidJsonBody_ThrowsException()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<AbpValidationException>(async () => await _packageAppService.Create(new CreatePackageDto
            {
                AssetId = 1,
                Name = "Test Package",
                Type = PackageType.Http,
                HttpPackageSettings = new HttpPackageSettingsDto
                {
                    Url = "http://localhost",
                    CycleSeconds = 60,
                    Encoding = HttpPackageBodyEncoding.Json,
                    Headers = "",
                    Body = "invalid json"
                }
            }));
        }

        [Fact]
        public async Task Create_HttpPackageInvalidXmlBody_ThrowsException()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act & Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<AbpValidationException>(async () => await _packageAppService.Create(new CreatePackageDto
            {
                AssetId = 1,
                Name = "Test Package",
                Type = PackageType.Http,
                HttpPackageSettings = new HttpPackageSettingsDto
                {
                    Url = "http://localhost",
                    CycleSeconds = 60,
                    Encoding = HttpPackageBodyEncoding.Xml,
                    Headers = "",
                    Body = "invalid xml"
                }
            }));
        }

        // TODO: GetStatistic

        // TODO: GetStatistics

        [Fact]
        public async Task Update_ValidPingSettings_Updated()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset().AddPingPackageWithStatus(Criticality.Healthy);
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await _packageAppService.Update(new UpdatePackageDto
            {
                Id = 1,
                Name = "Updated Package",
                PingPackageSettings = new PingPackageSettingsDto
                {
                    Host = "localhost",
                    CycleSeconds = 60
                }
            });
            await uow.CompleteAsync();

            // Assert
            using var evalUow = Resolve<IUnitOfWorkManager>().Begin();
            UsingDbContext(context =>
            {
                var package = context.Packages
                    .Include(p => p.PingPackageSettings)
                    .FirstOrDefault(p => p.Id == 1);
                Assert.NotNull(package);
                Assert.Equal(1, package.Id);
                Assert.Equal("Updated Package", package.Name);
                Assert.NotNull(package.PingPackageSettings);
                Assert.Equal("localhost", package.PingPackageSettings.Host);
            });
        }

        [Fact]
        public async Task Delete_PackageExists_Deleted()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset().AddPingPackageWithStatus(Criticality.Healthy);
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await _packageAppService.Delete(1);
            await uow.CompleteAsync();

            // Assert
            using var evalUow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _packageAppService.Get(1));
        }

        // TODO: GetPackageStatusUpdateBuckets

        // TODO: GetPackageStatusChangeBuckets
    }
}
