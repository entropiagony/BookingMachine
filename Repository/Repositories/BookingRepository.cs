using Common.Pagination;
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

        public async Task<PagedList<Booking>> GetAllBookingsAsync(int pageNumber, int pageSize)
        {
            var bookings = db.Bookings.Include(x => x.Floor)
                .Include(x => x.Employee).ThenInclude(x => x.Manager).OrderByDescending(x => x.Id).AsQueryable();

            return await PagedList<Booking>.CreateAsync(bookings, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Booking>> GetApprovedBookingsAsync(DateTime date, int floorId)
        {
            return await db.Bookings.Where(x => x.BookingDate.Date == date.Date
            && x.Status == BookingStatus.Approved && x.FloorId == floorId).ToListAsync();
        }

        public async Task<Booking> GetBookingAsync(int bookingId)
        {
            return await db.Bookings.Include(x => x.Floor).FirstOrDefaultAsync(x => x.Id == bookingId);
        }

        public async Task<PagedList<Booking>> GetEmployeeBookingsAsync(string employeeId, int pageNumber, int pageSize)
        {
            var bookings = db.Bookings.Include(x => x.Floor).Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.Id).AsQueryable();

            return await PagedList<Booking>.CreateAsync(bookings, pageNumber, pageSize);
        }

        public async Task<PagedList<Booking>> GetPendingManagerBookingsAsync(string managerId, int pageNumber, int pageSize)
        {
            var bookings = db.Bookings.Include(x => x.Employee).Include(x => x.Floor)
                .Where(x => x.Status == BookingStatus.Pending && x.ManagerId == managerId)
                .OrderByDescending(x => x.Id).AsQueryable();

            return await PagedList<Booking>.CreateAsync(bookings, pageNumber, pageSize);
        }

        public async Task<bool> HasAlreadyBookedWorkPlace(AppUser user, Booking booking)
        {
            return await db.Bookings.AnyAsync(x => x.BookingDate == booking.BookingDate &&
            x.EmployeeId == user.Id);
        }
    }
}
