using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Common.Exceptions;
using Domain.Entities;
using Domain.Enums;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BookingMachine.Tests
{
    public class AccountServiceTests
    {
        [Fact]
        public async Task UpdateAccountAsync_UserDoesntExist_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var tokenService = A.Fake<ITokenService>();
            var mapper = A.Fake<IMapper>();

            var userId = "NON EXISTING USER ID";
            var updateUserDto = new UpdateUserDto();
            AppUser user = null;

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);

            var sut = new AccountService(unitOfWork, tokenService, mapper);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.UpdateAccountAsync(userId, updateUserDto));
            Assert.Equal("User doesn't exist", exception.Message);

        }

        [Fact]
        public async Task UpdateAccountAsync_ResultNotSucceeded_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var tokenService = A.Fake<ITokenService>();
            var mapper = A.Fake<IMapper>();

            var userId = "EXISTING USER ID";
            var updateUserDto = new UpdateUserDto();
            AppUser user = new AppUser { Id = userId };

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);

            var sut = new AccountService(unitOfWork, tokenService, mapper);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.UpdateAccountAsync(userId, updateUserDto));
            Assert.Equal("Failed to update user", exception.Message);
        }

    }
}
