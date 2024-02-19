using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Caching;
using CoMon.Images;
using CoMon.Images.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Assets
{
    [AbpAuthorize]
    public class ImageAppService(IRepository<Image, long> imageRepository, IObjectMapper mapper, ImageCache imageCache) : CoMonAppServiceBase
    {
        private readonly IRepository<Image, long> _imageRepository = imageRepository;
        private readonly IObjectMapper _mapper = mapper;
        private readonly ImageCache _imageCache = imageCache;

        public async Task<ImageDto> GetTitleImageForAsset(long assetId)
        {
            var imageId = await _imageRepository
                .GetAll()
                .Where(i => i.AssetId == assetId)
                .Select(i => i.Id)
                .FirstOrDefaultAsync();

            if (imageId == 0)
                return _mapper.Map<ImageDto>(null);

            return _mapper.Map<ImageDto>(await _imageCache.GetAsync(imageId));
        }

        public async Task<List<ImageDto>> GetImagesForAsset(long assetId)
        {
            var ids = await _imageRepository
                .GetAll()
                .Where(i => i.AssetId == assetId)
                .Select(i => i.Id)
                .ToListAsync();

            return _mapper.Map<List<ImageDto>>(ids.Select(_imageCache.Get).ToList());
        }

        public async Task Delete(long id)
        {
            await _imageRepository.DeleteAsync(id);
        }
    }
}
