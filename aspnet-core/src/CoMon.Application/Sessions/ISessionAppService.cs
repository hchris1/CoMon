using System.Threading.Tasks;
using Abp.Application.Services;
using CoMon.Sessions.Dto;

namespace CoMon.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
