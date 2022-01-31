using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using Common.Exceptions;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;


        public AccountService(IUnitOfWork unitOfWork, ITokenService tokenService,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.tokenService = tokenService;
            this.mapper = mapper;

        }

        public async Task<UserDto> CreateAccountAsync(RegisterDto registerDto)
        {
            if (await unitOfWork.UserManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
                throw new BadRequestException("Username already exists.");

            var user = mapper.Map<AppUser>(registerDto);
            
            var manager = await unitOfWork.UserManager.Users.
                Include(e => e.Employees).SingleOrDefaultAsync(u => u.Id == user.ManagerId);

            if (manager == null)
                throw new BadRequestException("Failed to get your manager");

            user.Manager = manager;
            var managerEmployee = new ManagerEmployee { Employee = user, Manager = manager };

            manager.Employees.Add(managerEmployee);

            var result = await unitOfWork.UserManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                throw new BadRequestException("Failed to create user");

            var roleResult = await unitOfWork.UserManager.AddToRoleAsync(user, "Employee");

            if (!roleResult.Succeeded)
                throw new BadRequestException("Failed to add to roles");

            return new UserDto
            {
                Username = user.UserName,
                Token = await tokenService.CreateTokenAsync(user),
            };
        }

        public async Task<UserInfoDto> GetUserInfoDto(string userId)
        {
            var user = await unitOfWork.UserManager.Users.Include(x => x.Manager).FirstOrDefaultAsync(x => x.Id ==userId);
            if (user == null)
                throw new NotFoundException("Can't find specified user");

            return mapper.Map<UserInfoDto>(user);
        }

        public async Task<UserDto> LoginAsync(LoginDto loginDto)
        {
            var user = await unitOfWork.UserManager.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null)
                throw new BadRequestException("Invalid username");

            var result = await unitOfWork.SignInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                throw new BadRequestException("Invalid login or password");

            return new UserDto
            {
                Username = user.UserName,
                Token = await tokenService.CreateTokenAsync(user)
            };
        }

        public async Task UpdateAccountAsync(string userId, UpdateUserDto updateUserDto)
        {
            var user = await unitOfWork.UserManager.FindByIdAsync(userId);

            mapper.Map(updateUserDto, user);

            var result = await unitOfWork.UserManager.UpdateAsync(user);

            if (result.Succeeded)
                return;

            throw new BadRequestException("Failed to update user");
        }


    }
}
