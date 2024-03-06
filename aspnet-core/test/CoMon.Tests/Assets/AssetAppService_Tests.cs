using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Runtime.Validation;
using CoMon.Assets;
using CoMon.Assets.Dtos;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CoMon.Tests.Assets
{
    public class AssetAppService_Tests : CoMonTestBase
    {
        private readonly AssetAppService _assetAppService;

        public AssetAppService_Tests()
        {
            _assetAppService = Resolve<AssetAppService>();
        }

        [Fact]
        public async Task Get_FilledAssetsTable_ReturnsAssetDto()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            AssetDto result;
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                result = await _assetAppService.Get(1);
                await uow.CompleteAsync();
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task Get_EmptyAssetsTable_ThrowsNotFoundException()
        {
            // Nothing to arrange

            // Act and Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _assetAppService.Get(1234));
        }

        [Fact]
        public async Task GetAllPreviews_FilledAssetsTable_ReturnsList()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            List<AssetPreviewDto> result;
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                result = await _assetAppService.GetAllPreviews();
            }

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task Create_AssetForRoot_CreatesAsset()
        {
            // Arrange
            var input = new CreateAssetDto()
            {
                Name = "Test Asset",
                Description = "Test Description"
            };

            // Act
            long id;
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                id = await _assetAppService.Create(input);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var asset = context.Assets.Find(id);
                Assert.NotNull(asset);
                Assert.Equal(input.Name, asset.Name);
                Assert.Equal(input.Description, asset.Description);
            });
        }

        [Fact]
        public async Task Create_AssetInGroup_CreatesAsset()
        {
            // Arrange
            var arrangedGroup = EntityFactory.CreateGroup();
            UsingDbContext(context => context.Groups.Add(arrangedGroup));
            var input = new CreateAssetDto()
            {
                Name = "Test Asset",
                Description = "Test Description",
                GroupId = 1
            };

            // Act
            long id;
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                id = await _assetAppService.Create(input);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var asset = context.Assets.Find(id);
                Assert.NotNull(asset);
                Assert.Equal(input.Name, asset.Name);
                Assert.Equal(input.Description, asset.Description);
                Assert.Equal(input.GroupId, asset.GroupId);
            });
        }

        [Fact]
        public async Task Create_GroupDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var input = new CreateAssetDto()
            {
                Name = "Test Asset",
                Description = "Test Description",
                GroupId = 1
            };

            // Act and Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _assetAppService.Create(input));
        }

        [Fact]
        public async Task UpdateName_AssetExists_UpdatesName()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));
            var newName = "New Name";

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await _assetAppService.UpdateName(1, newName);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var asset = context.Assets.Find((long)1);
                Assert.NotNull(asset);
                Assert.Equal(newName, asset.Name);
            });
        }

        [Fact]
        public async Task UpdateName_AssetDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var newName = "New Name";

            // Act and Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _assetAppService.UpdateName(1, newName));
        }

        [Fact]
        public async Task UpdateName_NewNameIsWhiteSpace_ThrowsAbpValidationException()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));
            var newName = " ";

            // Act and Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<AbpValidationException>(() => _assetAppService.UpdateName(1, newName));
        }

        [Fact]
        public async Task UpdateDescription_AssetExists_UpdatesDescription()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));
            var newDescription = "New Description";

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await _assetAppService.UpdateDescription(1, newDescription);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var asset = context.Assets.Find((long)1);
                Assert.NotNull(asset);
                Assert.Equal(newDescription, asset.Description);
            });
        }

        [Fact]
        public async Task UpdateDescription_AssetDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var newDescription = "New Description";

            // Act and Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _assetAppService.UpdateDescription(1, newDescription));
        }

        [Fact]
        public async Task UpdateGroup_AssetExists_UpdatesGroup()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            var arrangedGroup = EntityFactory.CreateGroup();
            UsingDbContext(context =>
            {
                context.Assets.Add(arrangedAsset);
                context.Groups.Add(arrangedGroup);
            });
            var newGroupId = 1;

            UsingDbContext(context =>
            {
                var assets = context.Assets.ToList();
            });

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await _assetAppService.UpdateGroup(1, newGroupId);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var asset = context.Assets.Find((long)1);
                Assert.NotNull(asset);
                Assert.Equal(newGroupId, asset.GroupId);
            });
        }

        [Fact]
        public async Task UpdateGroup_AssetExistsAndGroupIsNull_UpdatesGroup()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            var arrangedGroup = EntityFactory.CreateGroup();
            UsingDbContext(context =>
            {
                context.Assets.Add(arrangedAsset);
                context.Groups.Add(arrangedGroup);
            });
            var newGroupId = null as long?;

            UsingDbContext(context =>
            {
                var assets = context.Assets.ToList();
                var asset = assets.First();
                var groups = context.Groups.ToList();
            });

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await _assetAppService.UpdateDescription(1, "Abc");
                await _assetAppService.UpdateGroup(1, newGroupId);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var asset = context.Assets.Find((long)1);
                Assert.NotNull(asset);
                Assert.Null(asset.Group);
            });
        }

        [Fact]
        public async Task UpdateGroup_AssetDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var newGroupId = 1;

            // Act and Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _assetAppService.UpdateGroup(1, newGroupId));
        }

        [Fact]
        public async Task UpdateGroup_GroupDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));
            var newGroupId = 1;

            // Act and Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _assetAppService.UpdateGroup(1, newGroupId));
        }

        [Fact]
        public async Task Delete_AssetExists_DeletesAsset()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await _assetAppService.Delete(1);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var asset = context.Assets.Find((long)1);
                Assert.Null(asset);
            });
        }

        [Fact]
        public async Task UploadImage_AssetExists_UploadsImage()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Setup mock file using a memory stream
            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake Image";
            var fileName = "test.png";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.ContentType).Returns("image/png");

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await _assetAppService.UploadImage(1, fileMock.Object);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var asset = context.Assets.Find((long)1);
                Assert.NotNull(asset);
                Assert.NotEmpty(context.Images);
            });
        }

        [Fact]
        public async Task UploadImage_AssetDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake Image";
            var fileName = "test.png";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.ContentType).Returns("image/png");

            // Act and Assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _assetAppService.UploadImage(1, fileMock.Object));
        }

        [Fact]
        public async Task UploadImage_InvalidImageType_ThrowsAbpValidationException()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.ContentType).Returns("text/plain");

            // Act and Assert
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await Assert.ThrowsAsync<AbpValidationException>(() => _assetAppService.UploadImage(1, fileMock.Object));
            }
        }

        [Fact]
        public async Task UploadImage_InvalidImageSize_ThrowsAbpValidationException()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.Length).Returns(10000000);

            // Act and Assert
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await Assert.ThrowsAsync<AbpValidationException>(() => _assetAppService.UploadImage(1, fileMock.Object));
            }
        }
    }
}
