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
                Name = input.Name,
                Description = input.Description,
                Group = group
            };

            return await _assetRepository.InsertAndGetIdAsync(asset);
        }

        public async Task UpdateName(long id, string name)
        {
            var asset = await _assetRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("Asset not found.");

            asset.Name = name;
            await _assetRepository.UpdateAsync(asset);
        }

        public async Task UpdateDescription(long id, string description)
        {
            var asset = await _assetRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("Asset not found.");

            asset.Description = description;
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
            //var asset = await _assetRepository
            //    .GetAll()
            //    .Where(p => p.Id == id)
                //.Include(a => a.Images)
                //.Include(a => a.Packages)
                //.ThenInclude(p => p.PingPackageSettings)
                //.Include(a => a.Packages)
                //.ThenInclude(p => p.Statuses)
                //.ThenInclude(s => s.KPIs)
                //.Include(a => a.Packages)
                //.ThenInclude(p => p.Statuses)
                //.ThenInclude(s => s.Charts)
                //.ThenInclude(c => c.Series)
                //.ThenInclude(s => s.DataPoints)
                //.FirstOrDefaultAsync()
                //?? throw new EntityNotFoundException("Asset not found.");
            await _assetRepository.DeleteAsync(id);
        }

        private Image CreateImageFromFormFile(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                throw new AbpValidationException("Invalid file.");

            // Check that size of image is less than 2MB
            if (file.Length > 2 * 1024 * 1024)
                throw new AbpValidationException("Image size must be less than 5MB.");

            // Check if mime type is valid
            var mimeType = file.ContentType;
            if (mimeType != "image/jpeg" && mimeType != "image/png" && mimeType != "image/gif" && mimeType != "image/svg+xml")
                throw new AbpValidationException("Invalid mime type. Must be image/jpeg, image/png, image/gif, or image/svg.");

            using var stream = file.OpenReadStream();
            using var binaryReader = new BinaryReader(stream);
            var data = binaryReader.ReadBytes((int)file.Length);

            return new Image
            {
                MimeType = mimeType,
                Data = data,
                Size = file.Length
            };
        }

        public async Task UploadImage(long id, IFormFile file)
        {
            var image = CreateImageFromFormFile(file);

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
