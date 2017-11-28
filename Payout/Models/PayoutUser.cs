using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Payout
{
    public class PayoutUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime LastLogin { get; set; }
        public bool? IsOkta { get; set; }        
        public string CreatedBy { get; set; }
        public bool? IsDisabled { get; set; }
    }    
}