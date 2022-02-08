using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using Common.Exceptions;
using Domain.Enums;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IEmailService emailService;

        public ManagerService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.emailService = emailService;
        }

        public async Task ApproveBooking(int bookingId)
        {
            var booking = await unitOfWork.BookingRepository.GetBookingAsync(bookingId);
            if (booking == null)
                throw new NotFoundException("Booking with this id doesn't exist");

            var floor = await unitOfWork.FloorRepository.GetFloorAsync(booking.FloorId);

            if (booking.BookingDate < DateTime.Today)
                throw new BadRequestException("Booking date had expired, please decline");

            if (booking.Status != BookingStatus.Pending)
                throw new BadRequestException("This booking is already managed");

            var approvedBookings = await unitOfWork.BookingRepository.GetApprovedBookingsAsync(booking.BookingDate);

            if (approvedBookings.Any(x => x.WorkPlaceId == booking.WorkPlaceId))
                throw new BadRequestException("This workplace is already taken, please decline");

            if(approvedBookings.Count() >= floor.WorkPlaces.Count * 0.2)
                throw new BadRequestException("Due to COVID-19 restrictions you cannot approve this booking");

            booking.Status = BookingStatus.Approved;

            if (await unitOfWork.Complete())
                return;

            throw new BadRequestException("Failed to approve booking");
        }

        public async Task DeclineBooking(int bookingId, string reason)
        {
            if (string.IsNullOrEmpty(reason))
                throw new BadRequestException("You must provide a reason to decline");

            var booking = await unitOfWork.BookingRepository.GetBookingAsync(bookingId);
            var employee = await unitOfWork.UserManager.FindByIdAsync(booking.EmployeeId);
                       
            if (employee == null)
                throw new NotFoundException("Employee with this booking id doesn't exist");

            if (booking == null)
                throw new NotFoundException("Booking with this id doesn't exist");

            if (booking.Status != BookingStatus.Pending)
                throw new BadRequestException("This booking is already managed");

            booking.Status = BookingStatus.Declined;

            if (await unitOfWork.Complete())
            {
                // await emailService.SendEmailAsync(employee.Email, $"Booking {bookingId} declined", reason);
                return;
            }

            throw new BadRequestException("Failed to decline booking");
        }

        public async Task<IEnumerable<ManagerDto>> GetManagers()
        {
            var managers = await unitOfWork.ManagerRepository.GetManagersAsync();
            var managerDtos = mapper.Map<IEnumerable<ManagerDto>>(managers);

            return managerDtos;
        }
    }
}
