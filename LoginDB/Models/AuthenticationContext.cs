using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UtilsLib.Utils;

namespace LoginLib.Models
{
    public class AuthenticationContext : IdentityDbContext<ApplicationUser>
    {
        public AuthenticationContext() : base()
        {

        }

        public AuthenticationContext(DbContextOptions options):base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SubscriptionPackage> SubscriptionPackages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-CADISMJ; Database=UserDB; User Id=pedro;Password=Epiphone1!; MultipleActiveResultSets=True;");
        }
    }
}
