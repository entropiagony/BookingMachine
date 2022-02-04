using Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs
{
    public class EmployeeBookingDto
    {
        public int Id { get; set; }
        public int WorkPlaceId { get; set; }
        public DateTime BookingDate { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BookingStatus Status { get; set; }
    }
}
