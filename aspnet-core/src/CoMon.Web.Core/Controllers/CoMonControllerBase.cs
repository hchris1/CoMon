using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace CoMon.Controllers
{
    public abstract class CoMonControllerBase: AbpController
    {
        protected CoMonControllerBase()
        {
            LocalizationSourceName = CoMonConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
