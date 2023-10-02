using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketReservationSystemAPI.Models
{
    public class TrainSchedule
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string TrainId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }
    }
}
