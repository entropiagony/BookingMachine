using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> db, RoleManager<IdentityRole> roleManager)
        {
            if (await db.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeed.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            if (users == null) return;

            string[] roles = { "Admin", "Manager", "Employee" };

            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                   await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            foreach (var user in users)
            {
                await db.CreateAsync(user, "Pa$$w0rd");
                await db.AddToRoleAsync(user, "Employee");
                if (user.PhoneNumber == "+11111111")
                    await db.AddToRoleAsync(user, "Manager");
            }

            var admin = new AppUser
            {
                UserName = "admin",
                FirstName = "Admin",
                LastName = "Admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false
            };

            await db.CreateAsync(admin, "Pa$$w0rd");
            await db.AddToRolesAsync(admin, new[] { "Admin", "Manager" });

        }
    }
}
