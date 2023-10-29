using Microsoft.AspNetCore.Authentication;
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
#pragma warning disable CS0618 // Type or member is obsolete
        ISystemClock systemClock,
#pragma warning restore CS0618 // Type or member is obsolete
        ILoggerFactory loggerFactory,
        IUnitOfWorkManager unitOfWorkManager) : AbpSecurityStampValidator<Tenant, Role, User>(options, signInManager, systemClock, loggerFactory, unitOfWorkManager)
    {
    }
}
