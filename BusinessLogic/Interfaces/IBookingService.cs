using BusinessLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IBookingService
    {
        public Task<EmployeeBookingDto> CreateBookingAsync(CreateBookingDto bookingDto, string userId);
        public Task<IEnumerable<EmployeeBookingDto>> GetEmployeeBookingsAsync(string employeeId);
        public Task<AdminBookingDto> GetAdminBookingDto(int bookingId);
        public Task<ManagerBookingDto> GetManagerBookingDto(int bookingId);
        public Task<string> GetIdOfBookingManager(int bookingId);
        public Task<string> GetIdOfBookingEmployee(int bookingId);
        public Task<IEnumerable<ManagerBookingDto>> GetManagerPendingBookings(string managerId);
        public Task<IEnumerable<AdminBookingDto>> GetAdminBookingDtos();
    }
}
