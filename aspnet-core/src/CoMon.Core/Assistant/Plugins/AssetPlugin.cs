using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using CoMon.Assets;
using CoMon.Assets.Dtos;
using CoMon.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Assistant.Plugins
{
    [Description("Plugin to get information about assets and manage them.")]
    public class AssetPlugin(IRepository<Asset, long> _assetRepository, IRepository<Group, long> _groupRepository, IObjectMapper _mapper)
    {
        [KernelFunction, Description("Search for assets by name.")]
        public async Task<List<AssetDto>> SearchAssets(string name)
        {
            var assets = await _assetRepository
                .GetAll()
                .Where(a => a.Name.ToLower().Contains(name.ToLower()))
                .Include(a => a.Group.Parent.Parent)
                .Include(a => a.Packages)
                .ThenInclude(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<AssetDto>>(assets);
        }

        [KernelFunction, Description("Get an asset by its ID along with its group, packages and the latest status of the packages.")]
        public async Task<AssetDto> GetAssetById(int id)
        {
            var asset = await _assetRepository
                .GetAll()
                .Where(a => a.Id == id)
                .Include(a => a.Group.Parent.Parent)
                .Include(a => a.Packages)
                .ThenInclude(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .AsSplitQuery()
                .AsNoTracking()
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Asset not found.");

            return _mapper.Map<AssetDto>(asset);
        }

        [KernelFunction, Description("Create an asset. If the groupId is left null, the asset will be created in the root group.")]
        public async Task<long> Create(string name, string description = "", long? groupId = null)
        {
            Group group = null;
            if (groupId.HasValue)
                group = await _groupRepository.GetAsync(groupId.Value)
                    ?? throw new EntityNotFoundException("Group not found.");


            var asset = new Asset()
            {
                Name = name.Trim(),
                Description = description,
                Group = group
            };

            return await _assetRepository.InsertAndGetIdAsync(asset);
        }

        [KernelFunction, Description("Update the name of an asset.")]
        public async Task UpdateName(int id, string name)
        {
            var asset = await _assetRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("Asset not found.");

            asset.Name = name;
            await _assetRepository.UpdateAsync(asset);
        }

        [KernelFunction, Description("Update the description of an asset.")]
        public async Task UpdateDescription(int id, string description)
        {
            var asset = await _assetRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("Asset not found.");

            asset.Description = description;
            await _assetRepository.UpdateAsync(asset);
        }

        [KernelFunction, Description("Update the group of an asset to move it. When the groupId is null, it is moved to the root group.")]
        public async Task UpdateGroup(long id, long? groupId)
        {
            var asset = await _assetRepository
                .GetAll()
                .Include(a => a.Group)
                .Where(a => a.Id == id)
                .AsSplitQuery()
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Asset not found.");

            if (groupId == null)
                asset.Group = null;
            else
                asset.Group = (await _groupRepository.GetAsync(groupId.Value))
                    ?? throw new EntityNotFoundException("Group not found.");

            await _assetRepository.UpdateAsync(asset);
        }
    }
}
