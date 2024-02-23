using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Validation;
using CoMon.Assets;
using CoMon.Groups.Dtos;
using CoMon.Statuses.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Groups
{
    [AbpAuthorize]
    public class GroupAppService(IRepository<Group, long> groupRepository, IRepository<Asset, long> assetRepository, IObjectMapper objectMapper) : CoMonAppServiceBase
    {
        private readonly IRepository<Group, long> _groupRepository = groupRepository;
        private readonly IRepository<Asset, long> _assetRepository = assetRepository;
        private readonly IObjectMapper _objectMapper = objectMapper;

        /// <summary>
        /// Creates an artifical root group that contains all asset that don't belong to any group and all top groups.
        /// </summary>
        /// <returns>Root group</returns>
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

        public async Task<GroupDto> Get(long id)
        {
            var group = await _groupRepository
                .GetAll()
                .Where(g => g.Id == id)
                .Include(g => g.Parent.Parent)
                .Include(g => g.SubGroups)
                .Include(g => g.Assets)
                .AsSplitQuery()
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Group not found.");

            var groupDto = new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Parent = _objectMapper.Map<GroupPreviewDto>(group.Parent),
                AssetIds = group.Assets.Select(a => a.Id).ToList(),
                SubGroupIds = group.SubGroups.Select(g => g.Id).ToList()
            };

            return groupDto;
        }

        public async Task<GroupPreviewDto> GetPreview(long id)
        {
            var group = await _groupRepository
                    .GetAll()
                    .Where(g => g.Id == id)
                    .SingleOrDefaultAsync()
                    ?? throw new EntityNotFoundException("Group not found.");

            var groupDto = _objectMapper.Map<GroupPreviewDto>(group);

            groupDto.WorstStatus = _objectMapper.Map<StatusPreviewDto>((await GroupAppServiceHelper.GetLatestStatusesFromGroup(_groupRepository, id))
                .OrderByDescending(s => s.Criticality)
                .ThenByDescending(s => s.Time)
                .FirstOrDefault());

            return groupDto;
        }

        public async Task<List<GroupPreviewDto>> GetAllPreviews()
        {
            var groups = await _groupRepository
                .GetAll()
                .Include(g => g.Parent.Parent)
                .AsSplitQuery()
                .ToListAsync();

            return _objectMapper.Map<List<GroupPreviewDto>>(groups);
        }

        public async Task<long> Create(CreateGroupDto input)
        {
            var group = new Group()
            {
                Name = input.Name?.Trim()
            };

            if (string.IsNullOrWhiteSpace(input.Name))
                throw new AbpValidationException("Group name may not be empty.");

            if (input.ParentId == null)
                return await _groupRepository.InsertAndGetIdAsync(group);

            var parent = await _groupRepository
                .GetAll()
                .Where(g => g.Id == input.ParentId)
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Parent group not found.");

            group.Parent = parent;
            return await _groupRepository.InsertAndGetIdAsync(group);
        }

        public async Task Delete(long id)
        {
            var group = await _groupRepository
                .GetAll()
                .Where(g => g.Id == id)
                .Include(g => g.Parent.Parent)
                .Include(g => g.SubGroups)
                .Include(g => g.Assets)
                .AsSplitQuery()
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Group not found.");

            foreach (var subGroup in group.SubGroups.ToList())
            {
                subGroup.Parent = group.Parent;
                await _groupRepository.UpdateAsync(subGroup);
            }

            foreach (var asset in group.Assets.ToList())
            {
                asset.Group = group.Parent;
                await _assetRepository.UpdateAsync(asset);
            }

            await _groupRepository.DeleteAsync(id);
        }

        public async Task UpdateName(long id, string name)
        {
            var group = await _groupRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("Group not found.");

            group.Name = name?.Trim();

            if (string.IsNullOrWhiteSpace(group.Name))
                throw new AbpValidationException("Group name may not be empty.");

            await _groupRepository.UpdateAsync(group);
        }

        public async Task UpdateParent(long id, long? parentId)
        {
            if (parentId.HasValue && parentId.Value == id)
                throw new AbpValidationException("Group may not be its own parent.");

            var group = await _groupRepository
                .GetAll()
                .Include(g => g.Parent)
                .Where(g => g.Id == id)
                .AsSplitQuery()
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Group not found.");

            if (parentId == null)
                group.Parent = null;
            else
                group.Parent = await _groupRepository
                .GetAll()
                .Where(g => g.Id == parentId)
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Group not found.");

            await _groupRepository.UpdateAsync(group);
        }
    }
}
