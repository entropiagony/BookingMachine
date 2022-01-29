using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly UserManager<AppUser> userManager;

        public ManagerRepository(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IEnumerable<AppUser>> GetManagersAsync()
        {
            return await userManager.GetUsersInRoleAsync("Manager");
        }
    }
}
