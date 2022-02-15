using Common.Pagination;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IBookingRepository
    {
        public void CreateBooking(Booking booking);
        public Task<PagedList<Booking>> GetEmployeeBookingsAsync(string employeeId, int pageNumber, int pageSize);
        public Task<IEnumerable<Booking>> GetApprovedBookingsAsync(DateTime date, int floorId);
        public Task<bool> HasAlreadyBookedWorkPlace(AppUser user, Booking booking);
        public Task<Booking> GetBookingAsync(int bookingId);
        public Task<PagedList<Booking>> GetPendingManagerBookingsAsync(string managerId, int pageNumber, int pageSize);
        public Task<PagedList<Booking>> GetAllBookingsAsync(int pageNumber, int pageSize);
    }
}
