using Abp.Authorization;
using CoMon.Authorization.Roles;
using CoMon.Authorization.Users;

namespace CoMon.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
