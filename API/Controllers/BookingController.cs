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
        public async Task<ActionResult> CreateBooking(BookingDto bookingDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await bookingService.CreateBookingAsync(bookingDto, userId);
            return Ok();
        } 
    }
}
