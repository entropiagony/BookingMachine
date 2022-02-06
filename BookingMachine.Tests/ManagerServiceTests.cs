using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Services;
using Domain.Entities;
using FakeItEasy;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookingMachine.Tests
{
    public class ManagerServiceTests
    {
        [Fact]
        public async Task GetManagers_ValidParameters_ReturnsBookingDtos()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var mapper = A.Fake<IMapper>();
            var managers = A.CollectionOfFake<AppUser>(10);
            var managerDtos = A.CollectionOfFake<ManagerDto>(10);

            A.CallTo(() => unitOfWork.ManagerRepository.GetManagersAsync()).Returns(managers);
            A.CallTo(() => mapper.Map<IEnumerable<ManagerDto>>(managers)).Returns(managerDtos);

            var sut = new ManagerService(unitOfWork, mapper);

            var result = await sut.GetManagers();
            Assert.Equal(managerDtos, result);
            Assert.Equal(managerDtos.Count(), result.Count());
        }
    }
}
