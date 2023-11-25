using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using CoMon.Images;
using CoMon.Images.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Assets
{
    public class ImageAppService(IRepository<Image, long> imageRepository, IObjectMapper mapper) 
        : CoMonAppServiceBase, IImageAppService
    {
        private readonly IRepository<Image, long> _imageRepository = imageRepository;
        private readonly IObjectMapper _mapper = mapper;

        public async Task<ImageDto> GetTitleImageForAsset(long assetId)
        {
            var image = await _imageRepository
                .FirstOrDefaultAsync(i => i.AssetId == assetId);
            return _mapper.Map<ImageDto>(image);
        }

        public async Task<List<ImageDto>> GetImagesForAsset(long assetId)
        {
            var images = await _imageRepository
                .GetAll()
                .Where(i => i.AssetId == assetId)
                .ToListAsync();

            return _mapper.Map<List<ImageDto>>(images);
        }

        public async Task Delete(long id)
        {
            await _imageRepository.DeleteAsync(id);
        }
    }
}
