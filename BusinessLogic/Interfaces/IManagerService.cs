using BusinessLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IManagerService
    {
        public Task<IEnumerable<ManagerDto>> GetManagers();
        public Task ApproveBooking(int bookingId);
        public Task DeclineBooking(int bookingId, string reason);
    }
}
