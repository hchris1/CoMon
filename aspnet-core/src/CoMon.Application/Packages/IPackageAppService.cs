using CoMon.Packages.Dtos;
using System.Threading.Tasks;

namespace CoMon.Packages
{
    public interface IPackageAppService
    {
        Task<PackageDto> Get(long id);
        Task<long> Create(CreatePackageDto input);
        Task Update(UpdatePackageDto input);
        Task Delete(long id);
    }
}
