using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TicketReservationSystemAPI.Models.Enums;

namespace TicketReservationSystemAPI.Models
{
    public class Train
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public TrainType Type { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public IEnumerable<ObjectId>? Schedules { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsPublished { get; set; } = false;
    }
}
