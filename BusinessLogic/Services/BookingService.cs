using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using Common.Exceptions;
using Common.Pagination;
using Domain.Entities;
using Domain.Enums;
using Repository.Interfaces;

namespace BusinessLogic.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<EmployeeBookingDto> CreateBookingAsync(CreateBookingDto bookingDto, string userId)
        {
            bookingDto.BookingDate = bookingDto.BookingDate.Date;
            var user = await unitOfWork.UserManager.FindByIdAsync(userId);
            var floor = await unitOfWork.FloorRepository.GetFloorAsync(bookingDto.FloorId);
            var workPlace = await unitOfWork.WorkPlaceRepository.GetWorkPlaceAsync(bookingDto.WorkPlaceId);

            if (user == null)
                throw new NotFoundException("Can't find specified user");

            if (floor == null)
                throw new NotFoundException("Can't find specified floor");

            if (workPlace == null)
                throw new NotFoundException("Can't find specified work place");

            if (workPlace.FloorId != bookingDto.FloorId)
                throw new BadRequestException("Your workplace doesn't belong to your floor");

            if (user.ManagerId == null)
                throw new BadRequestException("You don't have a manager, so you can't book");

            var booking = mapper.Map<Booking>(bookingDto);

            booking.Status = BookingStatus.Pending;
            booking.ManagerId = user.ManagerId;
            booking.EmployeeId = user.Id;

            if (await unitOfWork.BookingRepository.HasAlreadyBookedWorkPlace(user, booking))
                throw new BadRequestException("You have already booked workplace for this date");

            var bookings = await unitOfWork.BookingRepository.GetApprovedBookingsAsync(booking.BookingDate);
            bookings = bookings.Where(x => x.FloorId == booking.FloorId);

            if (bookings.Any(x => x.WorkPlaceId == workPlace.Id))
                throw new BadRequestException("This work place is already booked");

            if (bookings.Count() >= floor.WorkPlaces.Count * 0.2)
                throw new BadRequestException("Due to COVID-19 restrictions you cannot book this");

            unitOfWork.BookingRepository.CreateBooking(booking);


            if (await unitOfWork.Complete())
            {
                var employeeBooking = mapper.Map<EmployeeBookingDto>(booking);
                employeeBooking.FloorNumber = floor.FloorNumber;
                return employeeBooking;
            }

            throw new BadRequestException("Bad request");
        }

        public async Task<AdminBookingDto> GetAdminBookingDto(int bookingId)
        {
            var booking = await unitOfWork.BookingRepository.GetBookingAsync(bookingId);

            if (booking == null)
                throw new NotFoundException("Booking doesn't exist");

            var floor = await unitOfWork.FloorRepository.GetFloorAsync(booking.FloorId);
            if (floor == null)
                throw new NotFoundException("Can't find specified floor");

            var employee = await unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId);
            if (employee == null)
                throw new NotFoundException("Employee doesn't exist");

            var manager = await unitOfWork.UserManager.FindByIdAsync(booking.ManagerId);
            if (manager == null)
                throw new NotFoundException("Manager doesn't exist");

            var adminBookingDto = mapper.Map<AdminBookingDto>(booking);

            adminBookingDto.ManagerFirstName = manager.FirstName;
            adminBookingDto.ManagerLastName = manager.LastName;
            adminBookingDto.EmployeeFirstName = employee.FirstName;
            adminBookingDto.EmployeeLastName = employee.LastName;

            return adminBookingDto;
        }

        public async Task<PagedList<AdminBookingDto>> GetAdminBookingDtos(int pageNumber, int pageSize)
        {
            var bookings = await unitOfWork.BookingRepository.GetAllBookingsAsync(pageNumber, pageSize);
            var adminBookingDtos = mapper.Map<IList<Booking>, IList<AdminBookingDto>>(bookings);

            for (int i = 0; i < adminBookingDtos.Count; i++)
            {
                adminBookingDtos[i].EmployeeFirstName = bookings[i].Employee.FirstName;
                adminBookingDtos[i].EmployeeLastName = bookings[i].Employee.LastName;
                adminBookingDtos[i].ManagerFirstName = bookings[i].Manager.FirstName;
                adminBookingDtos[i].ManagerLastName = bookings[i].Manager.LastName;
            }

            return new PagedList<AdminBookingDto>(adminBookingDtos, bookings.TotalCount, bookings.CurrentPage, bookings.PageSize);
        }

        public async Task<PagedList<EmployeeBookingDto>> GetEmployeeBookingsAsync(string employeeId, int pageNumber, int pageSize)
        {
            var employee = await unitOfWork.UserManager.FindByIdAsync(employeeId);
            if (employee == null)
                throw new NotFoundException("Specified employee doesn't exist");

            var bookings = await unitOfWork.BookingRepository.GetEmployeeBookingsAsync(employeeId , pageNumber, pageSize);
            var bookingDtos = mapper.Map<IList<Booking>, IList<EmployeeBookingDto>>(bookings);

            return new PagedList<EmployeeBookingDto>(bookingDtos, bookings.TotalCount, bookings.CurrentPage, bookings.PageSize);
        }

        public async Task<string> GetIdOfBookingEmployee(int bookingId)
        {
            var booking = await unitOfWork.BookingRepository.GetBookingAsync(bookingId);
            if (booking == null)
                throw new NotFoundException("Booking doesn't exist");

            return booking.EmployeeId;
        }

        public async Task<string> GetIdOfBookingManager(int bookingId)
        {
            var booking = await unitOfWork.BookingRepository.GetBookingAsync(bookingId);
            if (booking == null)
                throw new NotFoundException("Booking doesn't exist");

            return booking.ManagerId;
        }

        public async Task<ManagerBookingDto> GetManagerBookingDto(int bookingId)
        {
            var booking = await unitOfWork.BookingRepository.GetBookingAsync(bookingId);
            if (booking == null)
                throw new NotFoundException("Booking doesn't exist");

            var employee = await unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId);
            if (employee == null)
                throw new NotFoundException("Employee doesn't exist");

            var managerBookingDto = mapper.Map<ManagerBookingDto>(booking);

            managerBookingDto.EmployeeFirstName = employee.FirstName;
            managerBookingDto.EmployeeLastName = employee.LastName;

            return managerBookingDto;
        }

        public async Task<PagedList<ManagerBookingDto>> GetManagerPendingBookings(string managerId, int pageNumber, int pageSize)
        {
            var manager = await unitOfWork.UserManager.FindByIdAsync(managerId);

            if (manager == null)
                throw new NotFoundException("User with this id doesnt exist");

            if (!await unitOfWork.UserManager.IsInRoleAsync(manager, "Manager"))
                throw new UnauthorizedException("You are not in manager role");

            var bookings = await unitOfWork.BookingRepository.GetPendingManagerBookingsAsync(managerId, pageNumber, pageSize);
            var managerBookingDtos = mapper.Map<IList<Booking>, IList<ManagerBookingDto>>(bookings);

            for (int i = 0; i < managerBookingDtos.Count; i++)
            {
                managerBookingDtos[i].EmployeeFirstName = bookings[i].Employee.FirstName;
                managerBookingDtos[i].EmployeeLastName = bookings[i].Employee.LastName;
            }

            return new PagedList<ManagerBookingDto>(managerBookingDtos, bookings.TotalCount, bookings.CurrentPage, bookings.PageSize);
        }
    }
}
