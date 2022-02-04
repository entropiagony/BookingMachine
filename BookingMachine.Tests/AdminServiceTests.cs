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

        [Fact]
        public async Task EditUserRolesAsync_ExistingRoles_ThrowsBadRequestException_FailedToRemoveFromRoles()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var username = "employee";
            var newRoles = new[] { "Employee" };
            var actualRoles = new[] { "Employee", "Manager" };
            var returnUser = new AppUser { UserName = username };
            var successIdentityResult = new IdentityResult();
            successIdentityResult = IdentityResult.Success;
            var sut = new AdminService(unitOfWork);

            A.CallTo(() => unitOfWork.UserManager.FindByNameAsync(username)).Returns(returnUser);
            A.CallTo(() => unitOfWork.UserManager.GetRolesAsync(returnUser)).Returns(actualRoles);
            A.CallTo(() => unitOfWork.UserManager.AddToRolesAsync(A<AppUser>.Ignored, A<string[]>.Ignored))
                .Returns(Task.FromResult(successIdentityResult));
            A.CallTo(() => unitOfWork.UserManager.RemoveFromRolesAsync(returnUser, actualRoles.Except(newRoles)))
                .Returns(Task.FromResult(IdentityResult.Failed()));


            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.EditUserRolesAsync(username, newRoles));
            Assert.Equal("Failed to remove from roles", exception.Message);
        }

        [Fact]
        public async Task EditUserRolesAsync_ExistingRoles_ReturnsNewRoles()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var username = "employee";
            var newRoles = new[] { "Manager", "Employee" };
            var actualRoles = new[] { "Employee" };
            var returnUser = new AppUser { UserName = username };


            A.CallTo(() => unitOfWork.UserManager.FindByNameAsync(username)).Returns(returnUser);
            A.CallTo(() => unitOfWork.UserManager.GetRolesAsync(returnUser)).Returns(actualRoles);
            A.CallTo(() => unitOfWork.UserManager.AddToRolesAsync(returnUser, newRoles.Except(actualRoles)))
                .Returns(Task.FromResult(IdentityResult.Success));
            A.CallTo(() => unitOfWork.UserManager.RemoveFromRolesAsync(returnUser, actualRoles.Except(newRoles)))
                .Returns(Task.FromResult(IdentityResult.Success));

            var sut = new AdminService(unitOfWork);
            var resultRoles = await sut.EditUserRolesAsync(username, newRoles);
            Assert.Equal(newRoles, resultRoles);
            Assert.Equal(newRoles.Length, resultRoles.Count);
        }

        [Fact]
        public async Task plswork()
        {
            var username = "employee";
            var newRoles = new[] { "Manager", "Employee" };
            var actualRoles = new[] { "Employee" };
            var returnUser = new AppUser { UserName = username };
            var successIdentityResult = new IdentityResult();
            successIdentityResult = IdentityResult.Success;
            var unitOfWork = new Mock<IUnitOfWork>();
            var store = new Mock<IUserStore<AppUser>>();
            var mgr = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
            unitOfWork.SetupGet(x => x.UserManager).Returns(mgr.Object);
            unitOfWork.Setup(a => a.UserManager.FindByNameAsync(username)).ReturnsAsync(returnUser);
            unitOfWork.Setup(a => a.UserManager.GetRolesAsync(returnUser)).ReturnsAsync(actualRoles);
            unitOfWork.Setup(a => a.UserManager.AddToRolesAsync(returnUser, newRoles.Except(actualRoles)))
                .ReturnsAsync(successIdentityResult);
            unitOfWork.Setup(a => a.UserManager.RemoveFromRolesAsync(returnUser, actualRoles.Except(newRoles)))
                .Returns(Task.FromResult(IdentityResult.Success));

            var sut = new AdminService(unitOfWork.Object);
            var resultRoles = await sut.EditUserRolesAsync(username, newRoles);
            Assert.Equal(newRoles, resultRoles);
            Assert.Equal(newRoles.Length, resultRoles.Count);
        }

        
    }
}