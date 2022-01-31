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
        public Task<IEnumerable<Booking>> GetBookingsAsync();
        public Task<IEnumerable<Booking>> GetApprovedBookingsAsync(DateTime date);
        public Task<bool> HasAlreadyBookedWorkPlace(AppUser user, Booking booking);
    }
}
