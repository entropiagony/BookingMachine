using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        public int WorkPlaceId { get; set; }
        [Required]
        public DateTime BookingDate { get; set; }
        [Required]
        public int FloorId { get; set; }
    }
}
