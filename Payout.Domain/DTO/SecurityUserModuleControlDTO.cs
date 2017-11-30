using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payout.Domain
{
    public class SecurityUserModuleControlDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? ModuleControlId { get; set; }
    }
}
