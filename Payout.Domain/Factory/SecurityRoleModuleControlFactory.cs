using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payout.Domain
{
    public class SecurityRoleModuleControlFactory
    {
        public SecurityRoleModuleControlDTO Create(PAYOUTsecurityRoleModuleControl securityRoleModuleControl)
        {
            if (securityRoleModuleControl != null)
            {
                return new SecurityRoleModuleControlDTO()
                {
                    Id = securityRoleModuleControl.Id,
                    ModuleControlId = securityRoleModuleControl.ModuleControlId,
                    RoleId = securityRoleModuleControl.RoleId
                };
            }

            return new SecurityRoleModuleControlDTO();
        }
    }
}
