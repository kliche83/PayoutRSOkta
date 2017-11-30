using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payout.Domain
{
    public class SecurityModuleControlFactory
    {
        public SecurityModuleControlDTO Create(PAYOUTsecurityModuleControl securityModuleControl)
        {
            if (securityModuleControl != null)
            {
                return new SecurityModuleControlDTO()
                {
                    Id = securityModuleControl.Id,
                    ModuleId = securityModuleControl.ModuleId,
                    ControlAccessId = securityModuleControl.ControlAccessId
                };
            }

            return new SecurityModuleControlDTO();
        }
    }
}
