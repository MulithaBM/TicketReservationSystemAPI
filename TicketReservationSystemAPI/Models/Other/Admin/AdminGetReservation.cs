namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminGetReservation
    {
        public string Id { get; set; }
        public string TravelerId { get; set; }
        public Guid TrainId { get; set; }
        public Guid ScheduleId { get; set; }
        public DateOnly BookingDate { get; set; }
        public DateOnly ReservationDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public int Seats { get; set; }
        public bool IsCancelled { get; set; }
    }
}
