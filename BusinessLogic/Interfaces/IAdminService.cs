using BusinessLogic.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IAdminService
    {
        public Task<ICollection<UserWithRolesDto>> GetUsersWithRolesAsync();
        public Task<ICollection<string>> EditUserRolesAsync(string username, string[] roles);
        
    }
}
