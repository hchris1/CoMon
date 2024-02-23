using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Runtime.Validation;
using CoMon.Groups;
using CoMon.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoMon.Tests.Groups
{
    public class GroupAppService_Tests : CoMonTestBase
    {
        private readonly GroupAppService _groupAppService;

        public GroupAppService_Tests()
        {
            _groupAppService = Resolve<GroupAppService>();
        }

        [Fact]
        public async Task GetRoot_RootFilled_ReturnsRoot()
        {
            // Arrange
            var arrangedGroup = EntityFactory
                .CreateGroup()
                .AddSubGroup();
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context =>
            {
                context.Groups.Add(arrangedGroup);
                context.Assets.Add(arrangedAsset);
            });

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var root = await _groupAppService.GetRoot();

            // Assert
            Assert.NotNull(root);
            Assert.Equal(0, root.Id);
            Assert.Equal("Root", root.Name);
            Assert.NotEmpty(root.AssetIds);
            Assert.NotEmpty(root.SubGroupIds);
        }

        [Fact]
        public async Task Get_GroupWithAssetsAndSubGroups_ReturnsGroup()
        {
            // Arrange
            var arrangedGroup = EntityFactory
                .CreateGroup()
                .AddSubGroup()
                .AddAsset(EntityFactory.CreateAsset());
            UsingDbContext(context => context.Groups.Add(arrangedGroup));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var group = await _groupAppService.Get(1);

            // Assert
            Assert.NotNull(group);
            Assert.Equal(1, group.Id);
            Assert.Equal("Test Group", group.Name);
            Assert.NotEmpty(group.AssetIds);
            Assert.NotEmpty(group.SubGroupIds);
        }

        [Fact]
        public async Task Get_GroupDoesNotExist_ThrowsNotFoundException()
        {
            // Nothing to arrange

            // Act and assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _groupAppService.Get(1));
        }

        [Fact]
        public async Task GetPreview_GroupWithAssetsAndSubGroups_ReturnsGroupPreview()
        {
            // Arrange
            var criticality = Criticality.Alert;
            var arrangedGroup = EntityFactory
                .CreateGroup()
                .AddSubGroup()
                .AddAsset(EntityFactory.CreateAsset().AddPingPackageWithStatus(criticality));
            UsingDbContext(context => context.Groups.Add(arrangedGroup));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var group = await _groupAppService.GetPreview(1);

            // Assert
            Assert.NotNull(group);
            Assert.Equal(1, group.Id);
            Assert.Equal("Test Group", group.Name);
            Assert.NotNull(group.WorstStatus);
            Assert.Equal(criticality, group.WorstStatus.Criticality);
        }

        [Fact]
        public async Task GetPreview_GroupDoesNotExist_ThrowsNotFoundException()
        {
            // Nothing to arrange

            // Act and assert
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _groupAppService.GetPreview(1));
        }

        [Fact]
        public async Task GetAllPreviews_GroupsExist_ReturnsGroupPreviews()
        {
            // Arrange
            var arrangedGroup = EntityFactory.CreateGroup();
            UsingDbContext(context => context.Groups.Add(arrangedGroup));

            // Act
            using var uow = Resolve<IUnitOfWorkManager>().Begin();
            var groups = await _groupAppService.GetAllPreviews();

            // Assert
            Assert.NotEmpty(groups);
            Assert.Single(groups);
            Assert.Equal(1, groups.First().Id);
            Assert.Equal("Test Group", groups.First().Name);
        }

        [Fact]
        public async Task Create_ValidInput_CreatesGroup()
        {
            // Arrange
            var input = new CreateGroupDto
            {
                Name = "New Group"
            };

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var id = await _groupAppService.Create(input);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var group = context.Groups.FirstOrDefault(g => g.Name == "New Group");
                Assert.NotNull(group);
            });
        }

        [Fact]
        public async Task Create_NameWithWhitespace_CreatesGroupWithTrimmedName()
        {
            // Arrange
            var input = new CreateGroupDto
            {
                Name = "   New Group   "
            };

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var id = await _groupAppService.Create(input);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var group = context.Groups.FirstOrDefault(g => g.Name == "New Group");
                Assert.NotNull(group);
            });
        }

        [Fact]
        public async Task Create_InsertWithinGroup_CreatesGroup()
        {
            // Arrange
            var arrangedGroup = EntityFactory.CreateGroup();
            UsingDbContext(context => context.Groups.Add(arrangedGroup));
            var input = new CreateGroupDto
            {
                Name = "New Group",
                ParentId = 1
            };

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var id = await _groupAppService.Create(input);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var group = context.Groups.FirstOrDefault(g => g.Name == "New Group");
                Assert.NotNull(group);
                Assert.Equal(1, group.ParentId);
            });
        }

        [Fact]
        public async Task Create_InvalidInput_ThrowsException()
        {
            // Arrange
            var input = new CreateGroupDto
            {
                Name = null
            };

            // Act and assert
            await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                using var uow = Resolve<IUnitOfWorkManager>().Begin();
                var id = await _groupAppService.Create(input);
                await uow.CompleteAsync();
            });
        }

        [Fact]
        public async Task Create_ParentDoesNotExist_ThrowsException()
        {
            // Arrange
            var input = new CreateGroupDto
            {
                Name = "New Group",
                ParentId = 1
            };

            // Act and assert
            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                using var uow = Resolve<IUnitOfWorkManager>().Begin();
                var id = await _groupAppService.Create(input);
                await uow.CompleteAsync();
            });
        }

        [Fact]
        public async Task Delete_GroupWithSubGroupsAndAssets_DeletesGroup()
        {
            // Arrange
            var arrangedGroup = EntityFactory
                .CreateGroup()
                .AddSubGroup()
                .AddAsset(EntityFactory.CreateAsset());
            UsingDbContext(context => context.Groups.Add(arrangedGroup));

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await _groupAppService.Delete(1);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var group = context.Groups.FirstOrDefault(g => g.Id == 1);
                Assert.Null(group);
                var subGroup = context.Groups.FirstOrDefault(g => g.Id == 2);
                Assert.NotNull(subGroup);
                var asset = context.Assets.FirstOrDefault(a => a.Id == 1);
                Assert.NotNull(asset);
            });
        }

        [Fact]
        public async Task Delete_GroupDoesNotExist_ThrowsException()
        {
            // Nothing to arrange

            // Act and assert
            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                using var uow = Resolve<IUnitOfWorkManager>().Begin();
                await _groupAppService.Delete(1);
                await uow.CompleteAsync();
            });
        }



        [Fact]
        public async Task UpdateName_GroupExists_UpdatesName()
        {
            // Arrange
            var arrangedGroup = EntityFactory.CreateGroup();
            UsingDbContext(context => context.Groups.Add(arrangedGroup));

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await _groupAppService.UpdateName(1, "New Name");
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var group = context.Groups.FirstOrDefault();
                Assert.NotNull(group);
                Assert.Equal("New Name", group.Name);
            });
        }

        [Fact]
        public async Task UpdateName_GroupDoesNotExist_ThrowsException()
        {
            // Nothing to arrange

            // Act and assert
            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                using var uow = Resolve<IUnitOfWorkManager>().Begin();
                await _groupAppService.UpdateName(1, "New Name");
                await uow.CompleteAsync();
            });
        }

        [Fact]
        public async Task UpdateName_InvalidInput_ThrowsException()
        {
            // Arrange
            var arrangedGroup = EntityFactory.CreateGroup();
            UsingDbContext(context => context.Groups.Add(arrangedGroup));

            // Act and assert
            await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                using var uow = Resolve<IUnitOfWorkManager>().Begin();
                await _groupAppService.UpdateName(1, null);
                await uow.CompleteAsync();
            });
        }

        [Fact]
        public async Task UpdateName_WhitespaceInput_ThrowsException()
        {
            // Arrange
            var arrangedGroup = EntityFactory.CreateGroup();
            UsingDbContext(context => context.Groups.Add(arrangedGroup));

            // Act and assert
            await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                using var uow = Resolve<IUnitOfWorkManager>().Begin();
                await _groupAppService.UpdateName(1, "   ");
                await uow.CompleteAsync();
            });
        }

        [Fact]
        public async Task UpdateParent_GroupExists_UpdatesParent()
        {
            // Arrange
            var arrangedGroup = EntityFactory.CreateGroup();
            UsingDbContext(context => context.Groups.Add(arrangedGroup));

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await _groupAppService.UpdateParent(1, null);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                var group = context.Groups.FirstOrDefault();
                Assert.NotNull(group);
                Assert.Null(group.ParentId);
            });
        }

        [Fact]
        public async Task UpdateParent_GroupDoesNotExist_ThrowsException()
        {
            // Nothing to arrange

            // Act and assert
            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                using var uow = Resolve<IUnitOfWorkManager>().Begin();
                await _groupAppService.UpdateParent(1, null);
                await uow.CompleteAsync();
            });
        }

        [Fact]
        public async Task UpdateParent_ParentDoesNotExist_ThrowsException()
        {
            // Arrange
            var arrangedGroup = EntityFactory.CreateGroup();
            UsingDbContext(context => context.Groups.Add(arrangedGroup));

            // Act and assert
            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                using var uow = Resolve<IUnitOfWorkManager>().Begin();
                await _groupAppService.UpdateParent(1, 2);
                await uow.CompleteAsync();
            });
        }

        [Fact]
        public async Task UpdateParent_ParentIdEqualsGroupId_ThrowsException()
        {
            // Arrange
            var arrangedGroup = EntityFactory.CreateGroup();
            UsingDbContext(context => context.Groups.Add(arrangedGroup));

            // Act and assert
            await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                using var uow = Resolve<IUnitOfWorkManager>().Begin();
                await _groupAppService.UpdateParent(1, 1);
                await uow.CompleteAsync();
            });
        }
    }
}
