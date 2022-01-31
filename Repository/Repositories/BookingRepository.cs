using Domain;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DataContext db;

        public BookingRepository(DataContext db)
        {
            this.db = db;
        }

        public void CreateBooking(Booking booking)
        {
            db.Bookings.Add(booking);
        }

        public async Task<IEnumerable<Booking>> GetBookingsAsync()
        {
            return await db.Bookings.ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetApprovedBookingsAsync(DateTime date)
        {
            return await db.Bookings.Where(x => x.BookingDate.Date == date.Date
            || x.Status == BookingStatus.Approved).ToListAsync();
        }

        public async Task<bool> HasAlreadyBookedWorkPlace(AppUser user, Booking booking)
        {
            return await db.Bookings.AnyAsync(x => x.BookingDate == booking.BookingDate &&
            x.EmployeeId == user.Id && x.WorkPlaceId == booking.WorkPlaceId);
        }
    }
}
