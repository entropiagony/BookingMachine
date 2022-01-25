using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ManagerEmployee
    {
        [ForeignKey("ManagerId")]
        public AppUser Manager { get; set; }
        [ForeignKey("EmployeeId")]
        public AppUser Employee { get; set; }
        public string ManagerId { get; set; }
        public string EmployeeId { get; set; }
    }
}
