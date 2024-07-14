using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Validation;
using CoMon.Assets;
using CoMon.Groups;
using CoMon.Groups.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Assistant.Plugins
{
    [Description("Plugin to get information about groups and manage them.")]
    public class GroupPlugin(IRepository<Asset, long> _assetRepository, IRepository<Group, long> _groupRepository,
        IObjectMapper _mapper)
    {
        [KernelFunction, Description("Get the root group of the application. This contains the top groups and assets that don't belong to any group.")]
        public async Task<GroupDto> GetRoot()
        {
            return new GroupDto
            {
                Id = 0,
                Name = "Root",
                AssetIds = await _assetRepository
                    .GetAll()
                    .Where(a => a.Group == null)
                    .OrderBy(a => a.Name)
                    .Select(a => a.Id)
                    .ToListAsync(),
                SubGroupIds = await _groupRepository
                    .GetAll()
                    .OrderBy(g => g.Name)
                    .Where(g => g.Parent == null)
                    .Select(g => g.Id)
                    .ToListAsync()
            };
        }

        [KernelFunction, Description("Search for groups by name.")]
        public async Task<List<GroupDto>> SearchGroups(string name)
        {
            var groups = await _groupRepository
                .GetAll()
                .Where(g => g.Name.ToLower().Contains(name.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<GroupDto>>(groups);
        }

        [KernelFunction, Description("Get a group by its ID along with its parent group, child groups and the assets in the group.")]
        public async Task<GroupDto> GetGroupById(int id)
        {
            var group = await _groupRepository
                .GetAll()
                .Where(g => g.Id == id)
                .Include(g => g.Parent)
                .Include(g => g.SubGroups)
                .Include(g => g.Assets)
                .AsSplitQuery()
                .AsNoTracking()
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Group not found.");

            return _mapper.Map<GroupDto>(group);
        }

        [KernelFunction, Description("Create a new group. When the parentGroupId is left empty, it is created as in the root.")]
        public async Task<long> CreateGroup(string name, int? parentGroupId)
        {
            var group = new Group()
            {
                Name = name?.Trim()
            };

            if (string.IsNullOrWhiteSpace(group.Name))
                throw new AbpValidationException("Group name may not be empty.");

            if (parentGroupId == null)
                return await _groupRepository.InsertAndGetIdAsync(group);

            var parent = await _groupRepository
                .GetAll()
                .Where(g => g.Id == parentGroupId)
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Parent group not found.");

            group.Parent = parent;
            return await _groupRepository.InsertAndGetIdAsync(group);
        }

        [KernelFunction, Description("Update the name of a group.")]
        public async Task UpdateName(int id, string name)
        {
            var group = await _groupRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("Group not found.");

            group.Name = name;
            await _groupRepository.UpdateAsync(group);
        }

        [KernelFunction, Description("Update the parent group of a group. When the parentGroupId is null, it is moved to the root.")]
        public async Task UpdateParent(int id, int? parentGroupId)
        {
            if (parentGroupId.HasValue && parentGroupId.Value == id)
                throw new AbpValidationException("Group may not be its own parent.");

            var group = await _groupRepository
                .GetAll()
                .Include(g => g.Parent)
                .Where(g => g.Id == id)
                .AsSplitQuery()
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Group not found.");

            if (parentGroupId == null)
                group.Parent = null;
            else
                group.Parent = await _groupRepository
                .GetAll()
                .Where(g => g.Id == parentGroupId)
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Group not found.");

            await _groupRepository.UpdateAsync(group);
        }
    }
}
