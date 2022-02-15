using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ReportMachine.Domain.Enums;

namespace ReportMachine.BusinessLogic.DTOs
{
    public class ReportDto
    {
        public int Id { get; set; }
        public int WorkPlaceId { get; set; }
        public DateTime BookingDate { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public BookingStatus Status { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string ManagerFirstName { get; set; }
        public string ManagerLastName { get; set; }
        public int FloorNumber { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
