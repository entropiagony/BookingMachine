using BusinessLogic.Services;
using Common.Exceptions;
using Domain.Entities;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Moq;
using Repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookingMachine.Tests
{
    public class AdminServiceTests
    {
        [Fact]
        public async Task EditUserRolesAsync_NotExistingUsername_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var username = "NON EXISTING USER";
            var newRoles = new[] { "Employee", "Manager" };
            AppUser returnUser = null;
            A.CallTo(() => unitOfWork.UserManager.FindByNameAsync(username)).Returns(returnUser);
            var sut = new AdminService(unitOfWork);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.EditUserRolesAsync(username, newRoles));
            Assert.Equal("Could not find this user", exception.Message);
        }

        [Fact]
        public async Task EditUserRolesAsync_NotExistingRoles_ThrowsBadRequestException_FailedToAddToRoles()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var username = "employee";
            var newRoles = new[] { "role that doesn't exist", "Employee" };
            var actualRoles = new[] { "Employee" };
            var returnUser = new AppUser { UserName = username };


            A.CallTo(() => unitOfWork.UserManager.FindByNameAsync(username)).Returns(returnUser);
            A.CallTo(() => unitOfWork.UserManager.GetRolesAsync(returnUser)).Returns(actualRoles);
            A.CallTo(() => unitOfWork.UserManager.AddToRolesAsync(returnUser, newRoles.Except(actualRoles)))
                .Returns(IdentityResult.Failed());
            var sut = new AdminService(unitOfWork);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.EditUserRolesAsync(username, newRoles));
            Assert.Equal("Failed to add to roles", exception.Message);
        }
    }
}