using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payout.Domain
{
    public class SecurityControlAccessFactory
    {
        public SecurityControlAccessDTO Create(PAYOUTsecurityControlAccess securityControlAccess)
        {
            if (securityControlAccess != null)
            {
                return new SecurityControlAccessDTO()
                {
                    Id = securityControlAccess.Id,
                    Description = securityControlAccess.Description
                };
            }

            return new SecurityControlAccessDTO();
        }
    }
}
