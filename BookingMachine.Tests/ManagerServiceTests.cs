using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using BusinessLogic.RabbitMQ;
using BusinessLogic.Services;
using BusinessLogic.Utilities;
using Common.Exceptions;
using Domain.Entities;
using Domain.Enums;
using FakeItEasy;
using Repository.Interfaces;
using System;
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
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var emailService = A.Fake<IEmailService>();
            var managers = new List<AppUser>();
            var managerDtos = new List<ManagerDto>();

            A.CallTo(() => unitOfWork.ManagerRepository.GetManagersAsync()).Returns(managers);

            var sut = new ManagerService(unitOfWork, mapper,emailService, bus);

            var result = await sut.GetManagers();
            Assert.Equal(managerDtos, result);
            Assert.Equal(managerDtos.Count(), result.Count());
        }

        [Fact]
        public async Task ApproveBooking_InvalidBookingId_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var emailService = A.Fake<IEmailService>();
            var bus = A.Fake<IBus>();

            var bookingId = -777;
            Booking booking = null;

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);

            var sut = new ManagerService(unitOfWork, mapper,emailService, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.ApproveBooking(bookingId));
            Assert.Equal("Booking with this id doesn't exist", exception.Message);
        }

        [Fact]
        public async Task ApproveBooking_BookingHasExpired_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var emailService = A.Fake<IEmailService>();
            var bus = A.Fake<IBus>();

            var bookingId = -777;
            Booking booking = new Booking { BookingDate = DateTime.MinValue};

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);

            var sut = new ManagerService(unitOfWork, mapper, emailService, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.ApproveBooking(bookingId));
            Assert.Equal("Booking date had expired, please decline", exception.Message);
        }

        [Fact]
        public async Task ApproveBooking_BookingStatusNotPending_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var emailService = A.Fake<IEmailService>();
            var bus = A.Fake<IBus>();

            var bookingId = -777;
            Booking booking = new Booking { BookingDate = DateTime.MaxValue, Status = BookingStatus.Approved };

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);

            var sut = new ManagerService(unitOfWork, mapper, emailService, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.ApproveBooking(bookingId));
            Assert.Equal("This booking is already managed", exception.Message);
        }

        [Fact]
        public async Task ApproveBooking_WorkPlaceTaken_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var emailService = A.Fake<IEmailService>();
            var bus = A.Fake<IBus>();

            var bookingId = -777;
            Booking booking = new Booking { BookingDate = DateTime.MaxValue, Status = BookingStatus.Pending, FloorId = 5, WorkPlaceId = 1 };
            var approvedBookings = new List<Booking>();
            approvedBookings.Add(new Booking { WorkPlaceId = booking.WorkPlaceId});

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.BookingRepository.GetApprovedBookingsAsync(booking.BookingDate, A<int>.Ignored)).Returns(approvedBookings);

            var sut = new ManagerService(unitOfWork, mapper, emailService, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.ApproveBooking(bookingId));
            Assert.Equal("This workplace is already taken, please decline", exception.Message);
        }

        [Fact]
        public async Task ApproveBooking_CovidRestrictions_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var emailService = A.Fake<IEmailService>();
            var bus = A.Fake<IBus>();

            var bookingId = -777;
            Booking booking = new Booking { BookingDate = DateTime.MaxValue, Status = BookingStatus.Pending, FloorId = 5 };
            var approvedBookings = new List<Booking>();
            approvedBookings.Add(new Booking { FloorId = booking.FloorId, WorkPlaceId = 777 });
            var floor = new Floor { WorkPlaces = A.CollectionOfFake<WorkPlace>(2), Id = booking.FloorId };

            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(booking.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.BookingRepository.GetApprovedBookingsAsync(booking.BookingDate, floor.Id)).Returns(approvedBookings);

            var sut = new ManagerService(unitOfWork, mapper, emailService, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.ApproveBooking(bookingId));
            Assert.Equal("Due to COVID-19 restrictions you cannot approve this booking", exception.Message);
        }

        [Fact]
        public async Task ApproveBooking_UnitOfWorkDoesntComplete_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var emailService = A.Fake<IEmailService>();
            var bus = A.Fake<IBus>();

            var bookingId = -777;
            Booking booking = new Booking { BookingDate = DateTime.MaxValue, Status = BookingStatus.Pending, FloorId = 5 };
            var approvedBookings = new List<Booking>();
            var floor = new Floor { WorkPlaces = A.CollectionOfFake<WorkPlace>(2), Id = booking.FloorId };

            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(booking.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.BookingRepository.GetApprovedBookingsAsync(booking.BookingDate, booking.FloorId)).Returns(approvedBookings);
            A.CallTo(() => unitOfWork.Complete()).Returns(false);

            var sut = new ManagerService(unitOfWork, mapper, emailService, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.ApproveBooking(bookingId));
            Assert.Equal("Failed to approve booking", exception.Message);
        }

        [Fact]
        public async Task DeclineBooking_ReasonIsNullOrEmpty_ThrowsBadRequestException()
        {

            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var emailService = A.Fake<IEmailService>();
            var bus = A.Fake<IBus>();

            var bookingId = -777;
            var reason = "";

            var sut = new ManagerService(unitOfWork, mapper, emailService, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.DeclineBooking(bookingId,reason));
            Assert.Equal("You must provide a reason to decline", exception.Message);
        }

        [Fact]
        public async Task DeclineBooking_BookingDoesntExist_ThrowsNotFoundException()
        {

            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var emailService = A.Fake<IEmailService>();
            var bus = A.Fake<IBus>();

            var bookingId = -777;
            var reason = "YOU DESERVE THIS";
            Booking booking = null;

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);

            var sut = new ManagerService(unitOfWork, mapper, emailService, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.DeclineBooking(bookingId, reason));
            Assert.Equal("Booking with this id doesn't exist", exception.Message);
        }

        [Fact]
        public async Task DeclineBooking_EmployeeDoesntExist_ThrowsNotFoundException()
        {

            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var emailService = A.Fake<IEmailService>();
            var bus = A.Fake<IBus>();

            var bookingId = -777;
            var reason = "YOU DESERVE THIS";
            AppUser employee = null;
            Booking booking = new Booking();

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId)).Returns(employee);

            var sut = new ManagerService(unitOfWork, mapper, emailService, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.DeclineBooking(bookingId, reason));
            Assert.Equal("Employee with this booking id doesn't exist", exception.Message);
        }

        [Fact]
        public async Task DeclineBooking_BookingStatusNotPending_ThrowsBadRequestException()
        {

            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var emailService = A.Fake<IEmailService>();
            var bus = A.Fake<IBus>();

            var bookingId = -777;
            var reason = "YOU DESERVE THIS";
            AppUser employee = new AppUser();
            Booking booking = new Booking { Status = BookingStatus.Declined};

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId)).Returns(employee);

            var sut = new ManagerService(unitOfWork, mapper, emailService, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.DeclineBooking(bookingId, reason));
            Assert.Equal("This booking is already managed", exception.Message);
        }

        [Fact]
        public async Task DeclineBooking_UnitOfWorkDoesntComplete_ThrowsNotFoundException()
        {

            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var emailService = A.Fake<IEmailService>();
            var bus = A.Fake<IBus>();

            var bookingId = -777;
            var reason = "YOU DESERVE THIS";
            AppUser employee = new AppUser();
            Booking booking = new Booking { Status = BookingStatus.Pending };

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId)).Returns(employee);
            A.CallTo(() => unitOfWork.Complete()).Returns(false);

            var sut = new ManagerService(unitOfWork, mapper, emailService, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.DeclineBooking(bookingId, reason));
            Assert.Equal("Failed to decline booking", exception.Message);
        }
    }
}
