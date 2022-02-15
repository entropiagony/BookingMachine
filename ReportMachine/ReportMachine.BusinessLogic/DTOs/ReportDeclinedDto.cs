using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMachine.BusinessLogic.DTOs
{
    public class ReportDeclinedDto
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public DateTime ManagedDate { get; set; }
    }
}
