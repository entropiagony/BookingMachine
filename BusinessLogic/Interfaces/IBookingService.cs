using BusinessLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IBookingService
    {
        public Task<EmployeeBookingDto> CreateBookingAsync(CreateBookingDto bookingDto, string userId);
        public Task<IEnumerable<EmployeeBookingDto>> GetEmployeeBookingsAsync(string employeeId);
    }
}
