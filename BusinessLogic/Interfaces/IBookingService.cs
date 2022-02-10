using BusinessLogic.DTOs;
using Common.Pagination;
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
        public Task<PagedList<EmployeeBookingDto>> GetEmployeeBookingsAsync(string employeeId, int pageNumber, int pageSize);
        public Task<AdminBookingDto> GetAdminBookingDto(int bookingId);
        public Task<ManagerBookingDto> GetManagerBookingDto(int bookingId);
        public Task<string> GetIdOfBookingManager(int bookingId);
        public Task<string> GetIdOfBookingEmployee(int bookingId);
        public Task<PagedList<ManagerBookingDto>> GetManagerPendingBookings(string managerId, int pageNumber, int pageSize);
        public Task<PagedList<AdminBookingDto>> GetAdminBookingDtos(int pageNumber, int pageSize);
    }
}
