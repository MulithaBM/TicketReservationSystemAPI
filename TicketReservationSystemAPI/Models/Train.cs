using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketReservationSystemAPI.Models
{
    public class Train
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public int Seats { get; set; }
        public IEnumerable<ObjectId> ScheduleIds { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublished { get; set; }
    }
}
