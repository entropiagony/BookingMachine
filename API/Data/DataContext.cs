using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, string,
        IdentityUserClaim<string>, AppUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>,
        IdentityUserToken<string>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().HasMany(ur => ur.UserRoles).WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId).IsRequired();
            modelBuilder.Entity<AppRole>().HasMany(ur => ur.UserRoles).WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId).IsRequired();
        }
    }
}
