using API.Extensions;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class BookingController : BaseApiController
    {
        private readonly IBookingService bookingService;

        public BookingController(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeBookingDto>> CreateBooking(CreateBookingDto bookingDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = await bookingService.CreateBookingAsync(bookingDto, userId);
            return Ok(result);
        }
        
        [HttpGet("employee")]
        public async Task<ActionResult<IEnumerable<EmployeeBookingDto>>> GetBookingsAsEmployee([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var bookings = await bookingService.GetEmployeeBookingsAsync(userId, pageNumber, pageSize);
            Response.AddPaginationHeader(bookings.CurrentPage, bookings.PageSize, bookings.TotalCount, bookings.TotalPages);
            return Ok(bookings);
        }

        [HttpGet("manager")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<ManagerBookingDto>>> GetBookingsAsManager([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var managerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var bookings = await bookingService.GetManagerPendingBookings(managerId, pageNumber, pageSize);
            Response.AddPaginationHeader(bookings.CurrentPage, bookings.PageSize, bookings.TotalCount, bookings.TotalPages);
            return Ok(bookings);
        }

        [HttpGet("admin")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<IEnumerable<AdminBookingDto>>> GetBookingsAsAdmin([FromQuery] int pageNumber = 1, [FromQuery]int pageSize = 10)
        {
            var bookings = await bookingService.GetAdminBookingDtos(pageNumber,pageSize);
            Response.AddPaginationHeader(bookings.CurrentPage, bookings.PageSize, bookings.TotalCount, bookings.TotalPages);
            return Ok(bookings);
        }
    }
}
