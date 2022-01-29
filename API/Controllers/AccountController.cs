﻿using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            return await accountService.CreateAccountAsync(registerDto);
        }


        [HttpPost("login")]

        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            return await accountService.LoginAsync(loginDto);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdateUser(UpdateUserDto updateUserDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await accountService.UpdateAccountAsync(userId, updateUserDto);
            return NoContent();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserInfoDto>> GetUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await accountService.GetUserInfoDto(userId);
            return Ok(user);
        }
    }
}

