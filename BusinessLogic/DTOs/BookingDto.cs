using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs
{
    public class BookingDto
    {
        public int WorkPlaceId { get; set; }
        public DateTime BookingDate { get; set; }

        public int FloorId { get; set; }
    }
}
