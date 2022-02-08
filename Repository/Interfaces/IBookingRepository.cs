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
        public Task<IEnumerable<Booking>> GetEmployeeBookingsAsync(string employeeId);
        public Task<IEnumerable<Booking>> GetApprovedBookingsAsync(DateTime date);
        public Task<bool> HasAlreadyBookedWorkPlace(AppUser user, Booking booking);
        public Task<Booking> GetBookingAsync(int bookingId);
        public Task<IList<Booking>> GetPendingManagerBookingsAsync(string managerId);
        public Task<IList<Booking>> GetAllBookingsAsync();
    }
}
