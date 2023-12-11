using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using CoMon.Assets;
using CoMon.Assets.Dtos;
using CoMon.Groups.Dtos;
using CoMon.Statuses.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Groups
{
    public class GroupAppService(IRepository<Group, long> groupRepository, IRepository<Asset, long> assetRepository, IObjectMapper objectMapper)
        : CoMonAppServiceBase, IGroupAppService
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
            var group = new GroupDto
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

            return group;
        }

        public async Task<long> Create(CreateGroupDto input)
        {
            var group = new Group()
            {
                Name = input.Name.Trim()
            };

            // Insert in root
            if (input.ParentId == null || input.ParentId <= 0)
                return await _groupRepository.InsertAndGetIdAsync(group);

            var parent = await _groupRepository
                .GetAll()
                .Where(g => g.Id == input.ParentId)
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Parent group not found.");

            group.Parent = parent;
            return await _groupRepository.InsertAndGetIdAsync(group);
        }

        public async Task Delete(long id)
        {
            await _groupRepository.DeleteAsync(id);
        }

        public async Task UpdateName(long id, string name)
        {
            var group = await _groupRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("Group not found.");

            group.Name = name.Trim();
            await _groupRepository.UpdateAsync(group);
        }

        public async Task UpdateParent(long id, long? parentId)
        {
            var group = await _groupRepository
                .GetAll()
                .Include(g => g.Parent)
                .Where(g => g.Id == id)
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Group not found.");

            if (parentId == null)
                group.Parent = null;
            else
                group.Parent = await _groupRepository
                .GetAll()
                .Where(g => g.Id == parentId)
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Group not found.");

            await _groupRepository.UpdateAsync(group);
        }

        public async Task<List<GroupPreviewDto>> GetAll()
        {
            var groups = await _groupRepository
                .GetAll()
                .Include(g => g.Parent.Parent)
                .ToListAsync();

            return _objectMapper.Map<List<GroupPreviewDto>>(groups);
        }

        private async Task<List<StatusPreviewDto>> GetLatestStatusesFromGroup(long groupId)
        {
            var group = await _groupRepository
                .GetAll()
                .Where(g => g.Id == groupId)
                .Include(g => g.SubGroups)
                .Include(g => g.Assets)
                .ThenInclude(a => a.Packages)
                .ThenInclude(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .SingleOrDefaultAsync() ?? throw new EntityNotFoundException("Group not found");


            var statuses = _objectMapper.Map<List<StatusPreviewDto>>(group.Assets.Select(a => a.Packages).SelectMany(x => x).Select(p => p.Statuses).SelectMany(x => x));

            foreach (var subGroup in group.SubGroups)
            {
                statuses.AddRange(await GetLatestStatusesFromGroup(subGroup.Id));
            }

            return statuses;
        }

        public async Task<GroupDto> Get(long id)
        {
            var group = await _groupRepository
                .GetAll()
                .Where(g => g.Id == id)
                .Include(g => g.Parent.Parent)
                .Include(g => g.SubGroups)
                .Include(g => g.Assets)
                .FirstOrDefaultAsync();

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

            groupDto.WorstStatus = (await GetLatestStatusesFromGroup(id)).OrderByDescending(s => s.Criticality).ThenByDescending(s => s.Time).FirstOrDefault();

            return groupDto;
        }
    }
}
