using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs
{
    public class ManagerBookingDto
    {
        public int Id { get; set; }
        public int WorkPlaceId { get; set; }
        public DateTime BookingDate { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BookingStatus Status { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }

    }
}
