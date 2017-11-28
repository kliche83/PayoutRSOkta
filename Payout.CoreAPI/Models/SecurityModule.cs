using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payout.CoreAPI.Models
{
    public class SecurityModule
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
    }
}
