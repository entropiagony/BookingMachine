using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.RabbitMQ;
using BusinessLogic.Services;
using BusinessLogic.Utilities;
using Common.Exceptions;
using Common.Pagination;
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
            var bus = A.Fake<IBus>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();

            var employeeId = "NON EXISTING EMPLOYEE ID";
            AppUser employee = null;

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(employeeId)).Returns(employee);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetEmployeeBookingsAsync(employeeId, 1, 10));
            Assert.Equal("Specified employee doesn't exist", exception.Message);
        }

        [Fact]
        public async Task GetEmployeeBookingsAsync_ValidParameters_ReturnsEmployeeBookingDtos()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var employeeId = "VALID EMPLOYEE ID";
            var employee = new AppUser { Id = employeeId };
            var bookings = A.Fake<PagedList<Booking>>();
            var bookingDtos = A.Fake<PagedList<EmployeeBookingDto>>();

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(employeeId)).Returns(employee);
            A.CallTo(() => unitOfWork.BookingRepository.GetEmployeeBookingsAsync(employeeId, 1, 10)).Returns(bookings);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var result = await sut.GetEmployeeBookingsAsync(employeeId, 1, 10);
            Assert.Equal(bookingDtos, result);
            Assert.Equal(bookingDtos.Count(), result.Count());
        }

        [Fact]
        public async Task CreateBookingAsync_NonExistingUser_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var userId = "NON EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = null;

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("Can't find specified user", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_NonExistingFloor_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId };
            Floor floor = null;

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("Can't find specified floor", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_NonExistingWorkPlace_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = null;

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("Can't find specified work place", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_WorkPlaceDoesntBelongToFloor_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = new WorkPlace { FloorId = 666 };

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("Your workplace doesn't belong to your floor", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_UserWithoutManager_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = new WorkPlace { FloorId = floor.Id };

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("You don't have a manager, so you can't book", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_UserHasAlreadyBookedWorkPlace_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId, ManagerId = "EXISTING MANAGER ID" };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = new WorkPlace { FloorId = floor.Id };
            var booking = mapper.Map<Booking>(bookingDto);


            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);
            A.CallTo(() => unitOfWork.BookingRepository.HasAlreadyBookedWorkPlace(user, A<Booking>.Ignored)).Returns(true);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("You have already booked workplace for this date", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_WorkPlaceAlreadyBooked_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId, ManagerId = "EXISTING MANAGER ID" };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = new WorkPlace { FloorId = floor.Id, Id = 1 };
            var booking = new Booking();
            var bookings = new List<Booking>();
            floor.WorkPlaces = A.CollectionOfDummy<WorkPlace>(10);
            bookings.Add(new Booking { WorkPlaceId = 1, FloorId = 13 });

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);
            A.CallTo(() => unitOfWork.BookingRepository.HasAlreadyBookedWorkPlace(user, A<Booking>.Ignored)).Returns(false);
            A.CallTo(() => unitOfWork.BookingRepository.GetApprovedBookingsAsync(bookingDto.BookingDate, floor.Id)).Returns(bookings);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("This work place is already booked", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_CovidWorkPlaceRestrictionsViolated_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId, ManagerId = "EXISTING MANAGER ID" };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = new WorkPlace { FloorId = floor.Id, Id = 10 };
            var booking = new Booking();
            var bookings = new List<Booking>();
            bookings.Add(new Booking { FloorId = 13 });
            bookings.Add(new Booking { FloorId = 13 });
            floor.WorkPlaces = A.CollectionOfDummy<WorkPlace>(10);

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);
            A.CallTo(() => unitOfWork.BookingRepository.HasAlreadyBookedWorkPlace(user, booking)).Returns(false);
            A.CallTo(() => unitOfWork.BookingRepository.GetApprovedBookingsAsync(bookingDto.BookingDate, floor.Id)).Returns(bookings);


            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("Due to COVID-19 restrictions you cannot book this", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_UnitOfWorkDoesntComplete_ThrowsBadRequestException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId, ManagerId = "EXISTING MANAGER ID" };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = new WorkPlace { FloorId = floor.Id, Id = 10 };
            var booking = new Booking();
            var bookings = new List<Booking>();
            floor.WorkPlaces = A.CollectionOfDummy<WorkPlace>(10);

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);
            A.CallTo(() => unitOfWork.BookingRepository.HasAlreadyBookedWorkPlace(user, booking)).Returns(false);
            A.CallTo(() => unitOfWork.BookingRepository.GetApprovedBookingsAsync(booking.BookingDate, floor.Id)).Returns(bookings);
            A.CallTo(() => unitOfWork.Complete()).Returns(false);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => sut.CreateBookingAsync(bookingDto, userId));
            Assert.Equal("Bad request", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_ValidParameters_ReturnsEmployeeBookingDto()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var userId = "EXISTING USER ID";
            var bookingDto = new CreateBookingDto { FloorId = 13, WorkPlaceId = 1, BookingDate = DateTime.Today };
            AppUser user = new AppUser { Id = userId, ManagerId = "EXISTING MANAGER ID" };
            Floor floor = new Floor { Id = bookingDto.FloorId };
            WorkPlace workPlace = new WorkPlace { FloorId = bookingDto.FloorId, Id = bookingDto.WorkPlaceId };
            var booking = new Booking { BookingDate = bookingDto.BookingDate.Date };
            var bookings = new List<Booking>();
            var employeeBookingDto = new EmployeeBookingDto { Status = booking.Status, BookingDate = booking.BookingDate, WorkPlaceId = workPlace.Id };
            floor.WorkPlaces = A.CollectionOfDummy<WorkPlace>(10);

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId)).Returns(workPlace);
            A.CallTo(() => unitOfWork.BookingRepository.HasAlreadyBookedWorkPlace(user, booking)).Returns(false);
            A.CallTo(() => unitOfWork.BookingRepository.GetApprovedBookingsAsync(booking.BookingDate, floor.Id)).Returns(bookings);
            A.CallTo(() => unitOfWork.Complete()).Returns(true);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var result = await sut.CreateBookingAsync(bookingDto, userId);
            Assert.Equal(bookingDto.BookingDate.Date, result.BookingDate);
            Assert.Equal(bookingDto.WorkPlaceId, result.WorkPlaceId);
            Assert.Equal(BookingStatus.Pending, result.Status);
        }

        [Fact]
        public async Task GetIdOfBookingManager_BookingDoesntExist_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            Booking booking = null;

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetIdOfBookingManager(bookingId));
            Assert.Equal("Booking doesn't exist", exception.Message);
        }

        [Fact]
        public async Task GetIdOfBookingManager_ValidParameters_ReturnsManagerId()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            var managerId = "YOUR MANAGER ID";
            Booking booking = new Booking { ManagerId = managerId };

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var result = await sut.GetIdOfBookingManager(bookingId);

            Assert.Equal(booking.ManagerId, result);
            Assert.Equal(managerId, result);
        }

        [Fact]
        public async Task GetAdminBookingDto_BookingDoesntExist_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            Booking booking = null;

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetAdminBookingDto(bookingId));
            Assert.Equal("Booking doesn't exist", exception.Message);
        }

        [Fact]
        public async Task GetManagerBookingDto_BookingDoesntExist_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            Booking booking = null;

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetManagerBookingDto(bookingId));
            Assert.Equal("Booking doesn't exist", exception.Message);
        }

        [Fact]
        public async Task GetAdminBookingDto_EmployeeDoesntExist_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            Booking booking = new Booking { Id = bookingId, EmployeeId = "employee dont exist" };
            AppUser employee = null;

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId)).Returns(employee);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetAdminBookingDto(bookingId));
            Assert.Equal("Employee doesn't exist", exception.Message);
        }

        [Fact]
        public async Task GetManagerBookingDto_EmployeeDoesntExist_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            Booking booking = new Booking { Id = bookingId, EmployeeId = "employee dont exist" };
            AppUser employee = null;

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId)).Returns(employee);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetManagerBookingDto(bookingId));
            Assert.Equal("Employee doesn't exist", exception.Message);
        }

        [Fact]
        public async Task GetAdminBookingDto_ManagerDoesntExist_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            Booking booking = new Booking { Id = bookingId, EmployeeId = "employee exists", ManagerId = "manager doesnt exist" };
            AppUser employee = new AppUser { Id = booking.EmployeeId };
            AppUser manager = null;

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId)).Returns(employee);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.ManagerId)).Returns(manager);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetAdminBookingDto(bookingId));
            Assert.Equal("Manager doesn't exist", exception.Message);
        }

        [Fact]
        public async Task GetAdminBookingDto_ValidParameters_ReturnsAdminBookingDto()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            Booking booking = new Booking { Id = bookingId, EmployeeId = "employee exists", ManagerId = "manager exists" };
            AppUser employee = new AppUser { Id = booking.EmployeeId, FirstName = "E.FName", LastName = "E.LName" };
            AppUser manager = new AppUser { Id = booking.ManagerId, FirstName = "M.FName", LastName = "M.LName" };

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId)).Returns(employee);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.ManagerId)).Returns(manager);

            var sut = new BookingService(unitOfWork, mapper, bus);
            var result = await sut.GetAdminBookingDto(bookingId);

            Assert.NotNull(result);
            Assert.Equal(employee.FirstName, result.EmployeeFirstName);
            Assert.Equal(employee.LastName, result.EmployeeLastName);
            Assert.Equal(manager.FirstName, result.ManagerFirstName);
            Assert.Equal(manager.LastName, result.ManagerLastName);
        }

        [Fact]
        public async Task GetManagerBookingDto_ValidParameters_ReturnsManagerBookingDto()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            Booking booking = new Booking { Id = bookingId, EmployeeId = "employee exists", ManagerId = "manager exists" };
            AppUser employee = new AppUser { Id = booking.EmployeeId, FirstName = "E.FName", LastName = "E.LName" };

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId)).Returns(employee);

            var sut = new BookingService(unitOfWork, mapper, bus);
            var result = await sut.GetManagerBookingDto(bookingId);

            Assert.NotNull(result);
            Assert.Equal(employee.FirstName, result.EmployeeFirstName);
            Assert.Equal(employee.LastName, result.EmployeeLastName);
        }

        [Fact]
        public async Task GetIdOfBookingEmployee_BookingDoesntExist_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = -666;
            Booking booking = null;

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetIdOfBookingEmployee(bookingId));
            Assert.Equal("Booking doesn't exist", exception.Message);
        }

        [Fact]
        public async Task GetIdOfBookingEmployee_ValidParameters_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            Booking booking = new Booking { Id = bookingId, EmployeeId = "employee id" };

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var result = await sut.GetIdOfBookingEmployee(bookingId);
            Assert.NotNull(result);
            Assert.Equal(booking.EmployeeId, result);
        }

        [Fact]
        public async Task GetAdminBookingDtos_ValidParameters_ReturnsAdminBookingDtos()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var pageNumber = 1;
            var pageSize = 10;
            var bookingsList = new List<Booking>();
            bookingsList.Add(new Booking
            {
                Employee = new AppUser { FirstName = "E.FName", LastName = "E.LName" },
                Manager = new AppUser { FirstName = "M.FName", LastName = "M.LName" }
            });
            var bookings = new PagedList<Booking>(bookingsList, bookingsList.Count, pageNumber, pageSize);

            A.CallTo(() => unitOfWork.BookingRepository.GetAllBookingsAsync(pageNumber, pageSize)).Returns(bookings);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var result = await sut.GetAdminBookingDtos(pageNumber, pageSize);
            Assert.NotNull(result);
            Assert.Equal(bookings.TotalCount, result.TotalCount);
            Assert.Equal(bookings.CurrentPage, result.CurrentPage);
            Assert.Equal(bookings.PageSize, result.PageSize);
            Assert.Equal(bookings.Count(), result.Count());
        }

        [Fact]
        public async Task GetManagerPendingBookings_ManagerDoesntExist_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var pageNumber = 1;
            var pageSize = 10;
            var managerId = "non existing manager id";
            AppUser manager = null;

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(managerId)).Returns(manager);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetManagerPendingBookings(managerId, pageNumber, pageSize));
            Assert.Equal("Manager with this id doesnt exist", exception.Message);
        }

        [Fact]
        public async Task GetManagerPendingBookings_ValidParameters_ReturnsManagerBookingDtos()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var pageNumber = 1;
            var pageSize = 10;
            var managerId = "existing manager id";
            AppUser manager = new AppUser { Id = managerId };
            var bookingsList = new List<Booking>();
            bookingsList.Add(new Booking
            {
                Employee = new AppUser { FirstName = "E.FName", LastName = "E.LName" }
            });
            var bookings = new PagedList<Booking>(bookingsList, bookingsList.Count, pageNumber, pageSize);

            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(managerId)).Returns(manager);
            A.CallTo(() => unitOfWork.BookingRepository.GetPendingManagerBookingsAsync(managerId, pageNumber, pageSize)).Returns(bookings);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var result = await sut.GetManagerPendingBookings(managerId, pageNumber, pageSize);
            Assert.NotNull(result);
            Assert.Equal(bookings.TotalCount, result.TotalCount);
            Assert.Equal(bookings.CurrentPage, result.CurrentPage);
            Assert.Equal(bookings.PageSize, result.PageSize);
            Assert.Equal(bookings.Count(), result.Count());
        }

        [Fact]
        public async Task GetReportDto_BookingDoesntExist_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = -666;
            Booking booking = null;

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetReportDto(bookingId));
            Assert.Equal("Booking doesn't exist", exception.Message);
        }

        [Fact]
        public async Task GetReportDto_FloorDoesntExist_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            Booking booking = new Booking { Id = bookingId, FloorId = 6 };
            Floor floor = null;

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(booking.FloorId)).Returns(floor);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetReportDto(bookingId));
            Assert.Equal("Can't find specified floor", exception.Message);
        }

        [Fact]
        public async Task GetReportDto_EmployeeDoesntExist_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            Booking booking = new Booking { Id = bookingId, FloorId = 6, EmployeeId = "e Id", ManagerId = "m Id" };
            Floor floor = new Floor { Id = booking.FloorId };
            AppUser employee = null;

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(booking.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId)).Returns(employee);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetReportDto(bookingId));
            Assert.Equal("Employee doesn't exist", exception.Message);
        }

        [Fact]
        public async Task GetReportDto_ManagerDoesntExist_ThrowsNotFoundException()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            Booking booking = new Booking { Id = bookingId, FloorId = 6, EmployeeId = "e Id", ManagerId ="m Id" };
            Floor floor = new Floor { Id = booking.FloorId };
            AppUser employee = new AppUser { FirstName = "E.FName", LastName = "E.LName" };
            AppUser manager = null;

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(booking.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId)).Returns(employee);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.ManagerId)).Returns(manager);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => sut.GetReportDto(bookingId));
            Assert.Equal("Manager doesn't exist", exception.Message);
        }

        [Fact]
        public async Task GetReportDto_ValidParameters_ReturnsReportDto()
        {
            var unitOfWork = A.Fake<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles>());
            var mapper = config.CreateMapper();
            var bus = A.Fake<IBus>();
            var bookingId = 666;
            Booking booking = new Booking { Id = bookingId, FloorId = 6, EmployeeId = "e Id", ManagerId = "m Id" };
            Floor floor = new Floor { Id = booking.FloorId };
            AppUser employee = new AppUser { FirstName = "E.FName", LastName = "E.LName" };
            AppUser manager = new AppUser { FirstName = "M.FName", LastName = "M.LName" };

            A.CallTo(() => unitOfWork.BookingRepository.GetBookingAsync(bookingId)).Returns(booking);
            A.CallTo(() => unitOfWork.FloorRepository.GetFloorAsync(booking.FloorId)).Returns(floor);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId)).Returns(employee);
            A.CallTo(() => unitOfWork.UserManager.FindByIdAsync(booking.ManagerId)).Returns(manager);

            var sut = new BookingService(unitOfWork, mapper, bus);

            var result = await sut.GetReportDto(bookingId);

            Assert.NotNull(result);
            Assert.Equal(employee.FirstName, result.EmployeeFirstName);
            Assert.Equal(employee.LastName, result.EmployeeLastName);
            Assert.Equal(manager.FirstName, result.ManagerFirstName);
            Assert.Equal(manager.LastName, result.ManagerLastName);
        }
    }
}
