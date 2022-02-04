using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using Common.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            bookingDto.BookingDate = bookingDto.BookingDate.AddHours(3).Date;
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
                throw new BadRequestException("You have already booked this place for this date");

            var bookings = await unitOfWork.BookingRepository.GetApprovedBookingsAsync(booking.BookingDate);

            if (bookings.Any(x => x.WorkPlaceId == workPlace.Id))
                throw new BadRequestException("This work place is already booked");

            if (bookings.Count() > floor.WorkPlaces.Count * 0.2)
                throw new BadRequestException("Due to COVID-19 restrictions you cannot book this");


            unitOfWork.BookingRepository.CreateBooking(booking);

            if (await unitOfWork.Complete())
                return mapper.Map<EmployeeBookingDto>(booking);

            throw new BadRequestException("Bad request");
            

        }

        public async Task<IEnumerable<EmployeeBookingDto>> GetEmployeeBookingsAsync(string employeeId)
        {
            var employee = await unitOfWork.UserManager.FindByIdAsync(employeeId);
            if (employee == null)
                throw new NotFoundException("Specified employee doesn't exist");

            var bookings = await unitOfWork.BookingRepository.GetEmployeeBookingsAsync(employeeId);
            var bookingDtos = mapper.Map<IEnumerable<Booking>, IEnumerable<EmployeeBookingDto>>(bookings);

            return bookingDtos;
        }
    }
}
