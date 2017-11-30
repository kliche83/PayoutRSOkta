using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payout.Domain
{
    public class SecurityModuleControlDTO
    {
        public int Id { get; set; }
        public int? ModuleId { get; set; }
        public int? ControlAccessId { get; set; }
    }
}
