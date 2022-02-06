using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Services;
using Common.Exceptions;
using Domain.Entities;
using Domain.Enums;
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
    public class BookingServiceTests
    {
        [Fact]
        public async Task GetEmployeeBookingsAsync_NonExistingEmployee_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var mapper = A.Fake<IMapper>();
            var employeeId = "NON EXISTING EMPLOYEE ID";
            AppUser employee = null;

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(employeeId)).Returns(employee);

            var sut = new BookingService(unitOfWork, mapper);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetEmployeeBookingsAsync(employeeId));
            Assert.Equal("Specified employee doesn't exist", exception.Message);
        }

        [Fact]
        public async Task GetEmployeeBookingsAsync_ValidParameters_ReturnsEmployeeBookingDtos()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var mapper = A.Fake<IMapper>();
            var employeeId = "VALID EMPLOYEE ID";
            var employee = new AppUser { Id = employeeId };
            var bookings = A.CollectionOfFake<Booking>(10);
            var bookingDtos = A.CollectionOfFake<EmployeeBookingDto>(10);

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(employeeId)).Returns(employee);
            A.CallTo(() => unitOfWork.BookingRepository.GetEmployeeBookingsAsync(employeeId)).Returns(bookings);
            A.CallTo(() => mapper.Map<IEnumerable<Booking>, IEnumerable<EmployeeBookingDto>>(bookings)).Returns(bookingDtos);

            var sut = new BookingService(unitOfWork, mapper);

            var result = await sut.GetEmployeeBookingsAsync(employeeId);
            Assert.Equal(bookingDtos, result);
            Assert.Equal(bookingDtos.Count(), result.Count());
        }

        [Fact]
        public async Task CreateBookingAsync_NonExistingUser_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var mapper = A.Fake<IMapper>();
            var userId = "NON EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = null;

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);

            var sut = new BookingService(unitOfWork, mapper);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("Can't find specified user", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_NonExistingFloor_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var mapper = A.Fake<IMapper>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId };
            Floor floor = null;

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);

            var sut = new BookingService(unitOfWork, mapper);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("Can't find specified floor", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_NonExistingWorkPlace_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var mapper = A.Fake<IMapper>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = null;

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);

            var sut = new BookingService(unitOfWork, mapper);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("Can't find specified work place", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_WorkPlaceDoesntBelongToFloor_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var mapper = A.Fake<IMapper>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = new WorkPlace { FloorId = 666 };

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);

            var sut = new BookingService(unitOfWork, mapper);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("Your workplace doesn't belong to your floor", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_UserWithoutManager_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var mapper = A.Fake<IMapper>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = new WorkPlace { FloorId = floor.Id };

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);

            var sut = new BookingService(unitOfWork, mapper);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("You don't have a manager, so you can't book", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_UserHasAlreadyBookedWorkPlace_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var mapper = A.Fake<IMapper>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId, ManagerId = "EXISTING MANAGER ID" };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = new WorkPlace { FloorId = floor.Id };
            var booking = new Booking();


            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);
            A.CallTo(() => mapper.Map<Booking>(bookingDto)).Returns(booking);
            

            var sut = new BookingService(unitOfWork, mapper);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("You have already booked workplace for this date", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_WorkPlaceAlreadyBooked_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var mapper = A.Fake<IMapper>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId, ManagerId = "EXISTING MANAGER ID" };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = new WorkPlace { FloorId = floor.Id };
            var booking = new Booking();
            var bookings = A.CollectionOfFake<Booking>(10);

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);
            A.CallTo(() => mapper.Map<Booking>(bookingDto)).Returns(booking);
            A.CallTo(() => unitOfWork.BookingRepository.HasAlreadyBookedWorkPlace(user, booking)).Returns(false);
            A.CallTo(() => unitOfWork.BookingRepository.GetApprovedBookingsAsync(booking.BookingDate)).Returns(bookings);

            var sut = new BookingService(unitOfWork, mapper);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("This work place is already booked", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_CovidWorkPlaceRestrictionsViolated_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var mapper = A.Fake<IMapper>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId, ManagerId = "EXISTING MANAGER ID" };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = new WorkPlace { FloorId = floor.Id };
            var booking = new Booking();
            var bookings = A.CollectionOfFake<Booking>(2);
            floor.WorkPlaces = A.CollectionOfDummy<WorkPlace>(10);

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);
            A.CallTo(() => mapper.Map<Booking>(bookingDto)).Returns(booking);
            A.CallTo(() => unitOfWork.BookingRepository.HasAlreadyBookedWorkPlace(user, booking)).Returns(false);
            A.CallTo(() => unitOfWork.BookingRepository.GetApprovedBookingsAsync(booking.BookingDate)).Returns(bookings);


            var sut = new BookingService(unitOfWork, mapper);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("Due to COVID-19 restrictions you cannot book this", exception.Message);
        }
    }
}
