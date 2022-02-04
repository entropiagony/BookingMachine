using Common;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, string,
        IdentityUserClaim<string>, AppUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>,
        IdentityUserToken<string>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Floor> Floors { get; set; }
        public DbSet<WorkPlace> WorkPlaces { get; set; }
        public DbSet<ManagerEmployee> Employees { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().HasMany(ur => ur.UserRoles).WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId).IsRequired();
            modelBuilder.Entity<AppRole>().HasMany(ur => ur.UserRoles).WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId).IsRequired();

            modelBuilder.Entity<ManagerEmployee>().HasKey(k => new { k.ManagerId, k.EmployeeId });
            modelBuilder.Entity<ManagerEmployee>().HasOne(e => e.Manager).WithMany(m => m.Employees).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>().Property(b => b.Status).HasConversion<string>();

            modelBuilder.ApplyUtcDateTimeConverter();
        }

    }
}
