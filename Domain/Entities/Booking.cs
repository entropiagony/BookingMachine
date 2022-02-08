using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int WorkPlaceId { get; set; }
        public WorkPlace WorkPlace { get; set; }
        public Floor Floor { get; set; }
        public int FloorId { get; set; }
        public DateTime BookingDate { get; set; }
        public string EmployeeId { get; set; }
        public string ManagerId { get; set; }
        [ForeignKey("EmployeeId")]
        public AppUser Employee { get; set; }
        [ForeignKey("ManagerId")]
        public AppUser Manager { get; set; }
        public BookingStatus Status = BookingStatus.Pending;
    }
}
