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
        public async Task<ActionResult<IEnumerable<EmployeeBookingDto>>> GetBookingsAsEmployee()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var bookings = await bookingService.GetEmployeeBookingsAsync(userId);
            return Ok(bookings);
        }
    }
}
