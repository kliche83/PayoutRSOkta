using Microsoft.AspNetCore.Identity;
using System;

namespace Payout.CoreAPI.Models
{
    public class PayoutUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime LastLogin { get; set; }
        public bool? IsOkta { get; set; }
        //public DateTime CreatedOn { get; set; }  --Cannot send this field because it contains default SQL script Value in the model
        public string CreatedBy { get; set; }
        public bool? IsDisabled { get; set; }
    }


    public class UpdateIdentity
    {
        public string id { get; set; }
        public string colName { get; set; }
        public dynamic colValue { get; set; }        
    }

    public class UserRoleViewModel
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public bool IsOkta { get; set; }
    }
}