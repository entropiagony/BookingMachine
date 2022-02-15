using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ReportMachine.Domain.Enums;
using System.Text.Json.Serialization;

namespace ReportMachine.Domain.Entities
{
    public class Report
    {
        [BsonId]
        public int Id { get; set; }
        public int WorkPlaceId { get; set; }
        public DateTime BookingDate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public BookingStatus Status { get; set; }
        public Worker Employee { get; set; }
        public Worker Manager { get; set; }
        public int FloorNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        [BsonIgnoreIfDefault]
        public DateTime ManagedDate { get; set; }
        [BsonIgnoreIfNull]
        public string Reason { get; set; }
    }
}
