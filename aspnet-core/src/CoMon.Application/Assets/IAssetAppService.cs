using CoMon.Assets.Dtos;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CoMon.Assets
{
    public interface IAssetAppService
    {

        Task<AssetDto> Get(long id);
        Task<long> Create(CreateAssetDto input);
        Task UpdateName(long id, string name);
        Task UpdateDescription(long id, string description);
        Task UpdateGroup(long id, long? groupId);
        Task Delete(long id);
        Task UploadImage(long id, IFormFile file);
    }
}
