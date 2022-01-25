using AutoMapper;
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
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;
        private readonly SignInManager<AppUser> signInManager;

        public AccountService(UserManager<AppUser> userManager, ITokenService tokenService,
            IMapper mapper, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
            this.signInManager = signInManager;
        }

        public async Task<UserDto> CreateAccountAsync(RegisterDto registerDto)
        {
            if (await userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
                throw new BadRequestException("Username already exists.");

            var user = mapper.Map<AppUser>(registerDto);

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                throw new BadRequestException("Failed to create user");

            var roleResult = await userManager.AddToRoleAsync(user, "Employee");

            if (!roleResult.Succeeded)
                throw new BadRequestException("Failed to add to roles");

            return new UserDto
            {
                Username = user.UserName,
                Token = await tokenService.CreateTokenAsync(user),
            };
        }

        public async Task<UserDto> LoginAsync(LoginDto loginDto)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null)
                throw new UnauthorizedException("Invalid username");

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                throw new UnauthorizedException("Invalid login or password");

            return new UserDto
            {
                Username = user.UserName,
                Token = await tokenService.CreateTokenAsync(user)
            };
        }

        public async Task<UserDto> UpdateAccountAsync()
        {
            throw new NotImplementedException();
        }
    }
}
