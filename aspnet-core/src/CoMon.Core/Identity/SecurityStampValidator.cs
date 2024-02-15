using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Abp.Authorization;
using CoMon.Authorization.Roles;
using CoMon.Authorization.Users;
using CoMon.MultiTenancy;
using Microsoft.Extensions.Logging;
using Abp.Domain.Uow;

namespace CoMon.Identity
{
    public class SecurityStampValidator(
        IOptions<SecurityStampValidatorOptions> options,
        SignInManager signInManager,
        ILoggerFactory loggerFactory,
        IUnitOfWorkManager unitOfWorkManager) : AbpSecurityStampValidator<Tenant, Role, User>(options, signInManager, loggerFactory, unitOfWorkManager)
    {
    }
}
