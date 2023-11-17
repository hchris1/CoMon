using Abp.Application.Services;
using CoMon.MultiTenancy.Dto;

namespace CoMon.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

