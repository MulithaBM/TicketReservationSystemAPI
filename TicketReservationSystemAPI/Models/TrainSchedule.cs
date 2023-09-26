using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketReservationSystemAPI.Models
{
    public class TrainSchedule
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId TrainId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int AvailableSeats { get; set; }
        public int Price { get; set; }
    }
}
