using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProductMaintenance.DataAccess
{
    public class IdentityAppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options)
            : base(options)
        {
        }
    }
}
