using BusinessLogic.Services;
using Common.Exceptions;
using Domain.Entities;
using FakeItEasy;
using Repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookingMachine.Tests
{
    public class FloorServiceTests
    {
        [Fact]
        public async Task CreateAsync_InvalidFloorNumber_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var floorNumber = -999;

            var sut = new FloorService(unitOfWork);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateAsync(floorNumber));
            Assert.Equal("Floor number should be greater or equal to zero", exception.Message);
        }

        [Fact]
        public async Task CreateAsync_UnitOfWorkDoesntComplete_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var floorNumber = 13;
            var createdFloor = new Floor { FloorNumber = floorNumber };
            A.CallTo(() => unitOfWork.FloorRepository.CreateAsync(floorNumber)).Returns(createdFloor);
            A.CallTo(() => unitOfWork.Complete()).Returns(false);

            var sut = new FloorService(unitOfWork);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateAsync(floorNumber));
            Assert.Equal("Failed to create floor", exception.Message);
        }

        [Fact]
        public async Task CreateAsync_ValidParameters_ReturnsNewFloor()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var floorNumber = 13;
            var createdFloor = new Floor { FloorNumber = floorNumber };
            A.CallTo(() => unitOfWork.FloorRepository.CreateAsync(floorNumber)).Returns(createdFloor);
            A.CallTo(() => unitOfWork.Complete()).Returns(true);

            var sut = new FloorService(unitOfWork);

            var newFloor = await sut.CreateAsync(floorNumber);
            Assert.Equal(createdFloor, newFloor);
            Assert.Equal(createdFloor.FloorNumber, newFloor.FloorNumber);
        }

        [Fact]
        public async Task DeleteAsync_UnitOfWorkDoesntComplete_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var id = 10;
            A.CallTo(() => unitOfWork.Complete()).Returns(false);

            var sut = new FloorService(unitOfWork);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.DeleteAsync(id));
            Assert.Equal("Failed to delete floor", exception.Message);

        }

        [Fact]
        public async Task DeleteAsync_ValidParameters_ExecutesDeleteFloorAsyncOnce()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var id = 10;

            A.CallTo(() => unitOfWork.Complete()).Returns(true);

            var sut = new FloorService(unitOfWork);

            await sut.DeleteAsync(id);
            A.CallTo(() => unitOfWork.FloorRepository.DeleteFloorAsync(A<int>.That.Matches(x => x == id))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetFloorsAsync_ValidParameters_ReturnsFloors()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var expectedResult = A.CollectionOfFake<Floor>(10);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorsWithWorkPlacesAsync()).Returns(expectedResult);

            var sut = new FloorService(unitOfWork);

            var actualResult = await sut.GetFloorsAsync();

            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedResult.Count(), actualResult.Count());
        }
    }
}
