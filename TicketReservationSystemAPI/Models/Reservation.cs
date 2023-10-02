using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketReservationSystemAPI.Models
{
    public class Reservation
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId TravelerId { get; set; }
        public ObjectId TrainId { get; set; }
        public ObjectId ScheduleId { get; set; }
        public int NoOfSeats { get; set; }
        public DateOnly ReservationDate { get; set; }
        public DateOnly BookingDate { get; set; }
        public bool IsCancelled { get; set; }
    }
}
