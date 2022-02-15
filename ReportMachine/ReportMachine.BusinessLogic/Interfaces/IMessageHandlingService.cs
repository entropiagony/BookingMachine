using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMachine.BusinessLogic.Interfaces
{
    public interface IMessageHandlingService
    {
        public Task HandleNewBooking(string message);
        public Task HandleApprovedBooking(string message);
        public Task HandleDeclinedBooking(string message);
    }
}
