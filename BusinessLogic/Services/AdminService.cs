using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using Common.Exceptions;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<AppUser> userManager;

        public AdminService(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<ICollection<string>> EditUserRolesAsync(string username, string[] roles)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
                throw new NotFoundException("Could not find this user");

            var userRoles = await userManager.GetRolesAsync(user);


            var result = await userManager.AddToRolesAsync(user, roles.Except(userRoles));

            if (!result.Succeeded)
                throw new BadRequestException("Failed to add to roles");

            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(roles));

            if (!result.Succeeded)
                throw new BadRequestException("Failed to remove from roles");

            return await userManager.GetRolesAsync(user);
        }

        public async Task<ICollection<UserWithRolesDto>> GetUsersWithRolesAsync()
        {
            var users = await userManager.Users.Include(r => r.UserRoles).ThenInclude(r => r.Role)
                      .OrderBy(u => u.UserName).Select(u => new UserWithRolesDto
                      {
                          Id = u.Id,
                          UserName = u.UserName,
                          Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                      }).ToListAsync();

            return users;
        }
    }
}
