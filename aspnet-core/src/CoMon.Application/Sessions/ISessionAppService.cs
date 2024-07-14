using System.Threading.Tasks;
using Abp.Application.Services;
using CoMon.Authorization.Dtos;

namespace CoMon.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
