using BusinessLogic.Services;
using Common.Exceptions;
using Domain.Entities;
using FakeItEasy;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BookingMachine.Tests
{
    public class WorkPlaceServiceTests
    {
        [Fact]
        public async Task CreateWorkPlacesAsync_NonExistingFloorId_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var quantity = 10;
            var floorId = -100;
            Floor floor = null;
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(floorId)).Returns(floor);

            var sut = new WorkPlaceService(unitOfWork);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.CreateWorkPlacesAsync(quantity, floorId));
            Assert.Equal("Specified floor doesn't exist", exception.Message);
        }

        [Fact]
        public async Task CreateWorkPlacesAsync_InvalidQuantity_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var quantity = -666;
            var floorId = 13;

            var sut = new WorkPlaceService(unitOfWork);
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateWorkPlacesAsync(quantity, floorId));
            Assert.Equal("Quantity should be greater or equal to zero", exception.Message);
        }

        [Fact]
        public async Task CreateWorkPlacesAsync_UnitOfWorkDoesntComplete_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var quantity = 5;
            var floorId = 13;
            A.CallTo(() => unitOfWork.Complete()).Returns(false);

            var sut = new WorkPlaceService(unitOfWork);
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateWorkPlacesAsync(quantity, floorId));
            Assert.Equal("Failed to create workplaces", exception.Message);
        }

        [Fact]
        public async Task CreateWorkPlacesAsync_ValidParameters_ReturnsWorkPlaces()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var quantity = 5;
            var floorId = 13;
            var expectedWorkPlaces = A.CollectionOfFake<WorkPlace>(quantity);
            A.CallTo(() => unitOfWork.Complete()).Returns(true);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.CreateWorkPlaces(quantity, floorId)).Returns(expectedWorkPlaces);

            var sut = new WorkPlaceService(unitOfWork);

            var actualWorkPlaces = await sut.CreateWorkPlacesAsync(quantity, floorId);
            Assert.Equal(expectedWorkPlaces, actualWorkPlaces);
            Assert.Equal(expectedWorkPlaces.Count(), actualWorkPlaces.Count());
        }

        [Fact]
        public async Task DeleteWorkPlaceAsync_UnitOfWorkDoesntComplete_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var id = 10;
            A.CallTo(() => unitOfWork.Complete()).Returns(false);

            var sut = new WorkPlaceService(unitOfWork);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.DeleteWorkPlaceAsync(id));
            Assert.Equal("Failed to delete workplace", exception.Message);

        }

        [Fact]
        public async Task DeleteAsync_ValidParameters_ExecutesDeleteFloorAsyncOnce()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var id = 10;

            A.CallTo(() => unitOfWork.Complete()).Returns(true);

            var sut = new WorkPlaceService(unitOfWork);

            await sut.DeleteWorkPlaceAsync(id);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.DeleteWorkPlaceAsync(A<int>.That.Matches(x => x == id))).MustHaveHappenedOnceExactly();
        }

    }
}
