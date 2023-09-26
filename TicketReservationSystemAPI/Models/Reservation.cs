using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketReservationSystemAPI.Models
{
    public class Reservation
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string TravelerId { get; set; }
        public ObjectId ScheduleId { get; set; }
        public int NoOfSeats { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime BookingDate { get; set; }
        public bool IsCancelled { get; set; }
    }
}
