using BusinessLogic.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IAccountService
    {
        public Task<UserDto> CreateAccountAsync(RegisterDto registerDto);
        public Task UpdateAccountAsync(string userId, UpdateUserDto updateUserDto);
        public Task<UserDto> LoginAsync(LoginDto loginDto);

        public Task<UserInfoDto> GetUserInfoDto(string userId);
    }
}
