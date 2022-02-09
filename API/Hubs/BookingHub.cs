using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace API.Hubs
{
    [Authorize]
    public class BookingHub : Hub
    {
        private readonly IBookingService bookingService;
        private readonly IManagerService managerService;

        public BookingHub(IBookingService bookingService, IManagerService managerService)
        {
            this.bookingService = bookingService;
            this.managerService = managerService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (Context.User.IsInRole("Admin"))
                await Groups.AddToGroupAsync(Context.ConnectionId, "AdminGroup");

            if (Context.User.IsInRole("Manager"))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Manager-{userId}");

            if (Context.User.IsInRole("Employee"))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Employee-{userId}");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (Context.User.IsInRole("Admin"))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminGroup");

            if (Context.User.IsInRole("Manager"))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Manager-{userId}");

            if (Context.User.IsInRole("Employee"))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Employee-{userId}");

            await base.OnDisconnectedAsync(exception);
        }

        public async Task CreateBooking(CreateBookingDto createBookingDto)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var employeeBookingDto = await bookingService.CreateBookingAsync(createBookingDto, userId);
                var adminBookingDto = await bookingService.GetAdminBookingDto(employeeBookingDto.Id);
                var managerBookingDto = await bookingService.GetManagerBookingDto(employeeBookingDto.Id);
                var managerId = await bookingService.GetIdOfBookingManager(employeeBookingDto.Id);

                await Clients.Caller.SendAsync("EmployeeNewBooking", employeeBookingDto);
                await Clients.Group("AdminGroup").SendAsync("AdminNewBooking", adminBookingDto);
                await Clients.Group($"Manager-{managerId}").SendAsync("ManagerNewBooking", managerBookingDto);
            }
            catch (Exception ex)
            {
                throw new HubException(ex.Message);
            }

        }

        [Authorize(Policy = "RequireManagerRole")]
        public async Task ApproveBooking(int bookingId)
        {
            var employeeId = await bookingService.GetIdOfBookingEmployee(bookingId);
            try
            {
                await managerService.ApproveBooking(bookingId);
                await Clients.Group("AdminGroup").SendAsync("AdminBookingApproved", new { bookingId, employeeId });
                await Clients.Group($"Employee-{employeeId}").SendAsync("EmployeeBookingApproved", bookingId);
            }
            catch (Exception ex)
            {
                throw new HubException(ex.Message);
            }
        }

        [Authorize(Policy = "RequireManagerRole")]
        public async Task DeclineBooking(int bookingId, string reason)
        {
            var employeeId = await bookingService.GetIdOfBookingEmployee(bookingId);
            try
            {
                await managerService.DeclineBooking(bookingId, reason);
                await Clients.Group("AdminGroup").SendAsync("AdminBookingDeclined", new { bookingId, employeeId });
                await Clients.Group($"Employee-{employeeId}").SendAsync("EmployeeBookingDeclined", new { bookingId, reason });
            }
            catch (Exception ex)
            {
                throw new HubException(ex.Message);
            }
        }
    }
}
