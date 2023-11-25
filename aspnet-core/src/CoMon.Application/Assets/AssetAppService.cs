using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Validation;
using CoMon.Assets.Dtos;
using CoMon.Groups;
using CoMon.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Assets
{
    public class AssetAppService : CoMonAppServiceBase, IAssetAppService
    {
        private readonly IObjectMapper _objectMapper;
        private readonly IRepository<Asset, long> _assetRepository;
        private readonly IRepository<Group, long> _groupRepository;

        public AssetAppService(IObjectMapper objectMapper, IRepository<Asset, long> assetRepository, 
            IRepository<Group, long> groupRepository)
        {
            _assetRepository = assetRepository;
            _objectMapper = objectMapper;
            _groupRepository = groupRepository;
        }

        public async Task<AssetDto> Get(long id)
        {
            var asset = await _assetRepository
                .GetAll()
                .Where(a => a.Id == id)
                .Include(a => a.Group.Parent.Parent)
                .Include(a => a.Packages)
                .ThenInclude(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Asset not found.");

            return _objectMapper.Map<AssetDto>(asset);
        }

        public async Task<List<AssetPreviewDto>> GetAll()
        {
            var assets = await _assetRepository
                    .GetAll()
                    .Include(a => a.Group.Parent.Parent)
                    .ToListAsync();

            return _objectMapper.Map<List<AssetPreviewDto>>(assets);    
        }

        public async Task<long> Create(CreateAssetDto input)
        {
            Group group = null;
            if (input.GroupId != null)
                group = await _groupRepository.GetAsync(input.GroupId.Value)
                    ?? throw new EntityNotFoundException("Group not found.");


            var asset = new Asset()
            {
                Name = input.Name.Trim(),
                Description = input.Description,
                Group = group
            };

            return await _assetRepository.InsertAndGetIdAsync(asset);
        }

        public async Task UpdateName(long id, string name)
        {
            var asset = await _assetRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("Asset not found.");

            asset.Name = name.Trim();
            await _assetRepository.UpdateAsync(asset);
        }

        public async Task UpdateDescription(long id, string description)
        {
            var asset = await _assetRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("Asset not found.");

            asset.Description = description.Trim();
            await _assetRepository.UpdateAsync(asset);
        }

        public async Task UpdateGroup(long id, long? groupId)
        {
            var asset = await _assetRepository
                .GetAll()
                .Include(a => a.Group)
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Asset not found.");

            if (groupId == null)
                asset.Group = null;
            else
                asset.Group = (await _groupRepository.GetAsync(groupId.Value))
                    ?? throw new EntityNotFoundException("Group not found.");

            await _assetRepository.UpdateAsync(asset);
        }

        public async Task Delete(long id)
        {
            await _assetRepository.DeleteAsync(id);
        }

        public async Task UploadImage(long id, IFormFile file)
        {
            var image = Image.CreateImageFromFormFile(file);

            var asset = await _assetRepository
                .GetAll()
                .Where(a => a.Id == id)
                .Include(a => a.Images)
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Asset not found.");

            asset.Images.Add(image);
            await _assetRepository.UpdateAsync(asset);
        }
    }
}
