using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Domain
{
    public class Seed
    {
        public static async Task SeedFloorsAndWorkPlaces(DataContext db)
        {
            if (await db.Floors.AnyAsync() || await db.WorkPlaces.AnyAsync()) return;

            var floors = new List<Floor>();
            var workPlaceCount = new List<int> { 20, 30, 45, 15, 50 };

            for (int i = 0; i < 5; i++)
            {
                var floor = new Floor
                {
                    FloorNumber = i,
                    WorkPlaces = new List<WorkPlace>()
                };

                var floorWorkPlaces = new List<WorkPlace>();

                for (int j = 0; j < workPlaceCount[i]; j++)
                {
                    var workPlace = new WorkPlace
                    {
                        Floor = floor
                    };
                    db.WorkPlaces.Add(workPlace);
                    floorWorkPlaces.Add(workPlace);
                }
                floor.WorkPlaces = floorWorkPlaces;
                db.Floors.Add(floor);
            }
            await db.SaveChangesAsync();
        }

        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("../Domain/UserSeed.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            if (users == null) return;

            var roles = new List<AppRole>
            {
                new AppRole{Name = "Admin" },
                new AppRole{Name = "Manager" },
                new AppRole{Name = "Employee" }
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Employee");
                if (user.PhoneNumber == "+11111111")
                    await userManager.AddToRoleAsync(user, "Manager");
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

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Manager" });

        }
    }
}
