using System.Threading.Tasks;
using Abp.Application.Services;
using CoMon.Authorization.Accounts.Dto;

namespace CoMon.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
