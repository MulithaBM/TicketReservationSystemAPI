// File name: Reservation.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>11/09/2023</created>
// <modified>11/10/2023</modified>

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketReservationSystemAPI.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public Guid TravelerId { get; set; }
        public Guid TrainId { get; set; }
        public Guid ScheduleId { get; set; }
        public int NoOfSeats { get; set; }
        public DateOnly ReservationDate { get; set; }
        public DateOnly BookingDate { get; set; }
        public bool IsCancelled { get; set; }
    }
}
