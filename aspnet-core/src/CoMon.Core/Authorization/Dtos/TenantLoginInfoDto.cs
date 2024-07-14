using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CoMon.MultiTenancy;

namespace CoMon.Authorization.Dtos
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}
