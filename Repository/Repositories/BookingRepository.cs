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

        public async Task<IList<Booking>> GetAllBookingsAsync()
        {
            return await db.Bookings.Include(x => x.Employee).ThenInclude(x => x.Manager).ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetApprovedBookingsAsync(DateTime date)
        {
            return await db.Bookings.Where(x => x.BookingDate.Date == date.Date
            && x.Status == BookingStatus.Approved).ToListAsync();
        }

        public async Task<Booking> GetBookingAsync(int bookingId)
        {
            return await db.Bookings.FirstOrDefaultAsync(x => x.Id == bookingId);
        }

        public async Task<IEnumerable<Booking>> GetEmployeeBookingsAsync(string employeeId)
        {
            return await db.Bookings.Where(x => x.EmployeeId == employeeId).ToListAsync();
        }

        public async Task<IList<Booking>> GetPendingManagerBookingsAsync(string managerId)
        {
            return await db.Bookings.Include(x => x.Employee)
                .Where(x => x.Status == BookingStatus.Pending && x.ManagerId == managerId).ToListAsync();
        }

        public async Task<bool> HasAlreadyBookedWorkPlace(AppUser user, Booking booking)
        {
            return await db.Bookings.AnyAsync(x => x.BookingDate == booking.BookingDate &&
            x.EmployeeId == user.Id);
        }
    }
}
