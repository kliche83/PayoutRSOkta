using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Payout.CoreAPI.Models;

namespace Payout.CoreAPI
{
    public class SecurityContext : IdentityDbContext<PayoutUser, PayoutRole, string>
    {
        public SecurityContext(DbContextOptions<SecurityContext> options)
            : base(options)
        {
        }
    }
}
