using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using CoMon.Assets;
using CoMon.Assets.Dtos;
using CoMon.Groups.Dtos;
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
            return new GroupDto
            {
                Id = 0,
                Name = "Root",
                Assets = _objectMapper.Map<List<AssetDto>>(await _assetRepository
                    .GetAll()
                    .Where(a => a.Group == null)
                    .OrderBy(a => a.Name)
                    .Include(a => a.Packages.OrderBy(p => p.Name))
                    .ThenInclude(p => p.Statuses
                        .OrderByDescending(s => s.Time)
                        .Take(1))
                    .ThenInclude(s => s.KPIs)
                    .ToListAsync()),
                SubGroups = _objectMapper.Map<List<GroupPreviewDto>>(await _groupRepository
                    .GetAll()
                    .OrderBy(g => g.Name)
                    .Where(g => g.Parent == null)
                    .ToListAsync())
            };
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

        public async Task<List<GroupDto>> GetAll()
        {
            var groups = await _groupRepository
                .GetAll()
                .Include(g => g.Parent.Parent)
                .ToListAsync();

            return _objectMapper.Map<List<GroupDto>>(groups);
        }

        public async Task<GroupDto> Get(long id)
        {
            var group = await _groupRepository
                .GetAll()
                .Where(g => g.Id == id)
                .Include(g => g.Parent.Parent)
                .Include(g => g.SubGroups)
                .Include(g => g.Assets.OrderBy(a => a.Name))
                .ThenInclude(a => a.Packages.OrderBy(p => p.Name))
                .ThenInclude(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .Include(g => g.Assets)
                .FirstOrDefaultAsync();

            return _objectMapper.Map<GroupDto>(group);
        }

        public async Task<GroupPreviewDto> GetPreview(long id)
        {
            var group = await _groupRepository
                    .GetAll()
                    .OrderBy(g => g.Name)
                    .Where(g => g.Parent == null)
                    .SingleOrDefaultAsync()
                    ?? throw new EntityNotFoundException("Group not found.");

            return _objectMapper.Map<GroupPreviewDto>(group);
        }
    }
}
