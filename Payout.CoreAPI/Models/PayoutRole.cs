using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Payout.CoreAPI.Models
{
    public class PayoutRole : IdentityRole
    {
        //public DateTime? CreatedOn { get; set; }  --Cannot send this field because it contains default SQL script Value in the model
        public string CreatedBy { get; set; }
        public bool? IsDisabled { get; set; }           

    }
}
