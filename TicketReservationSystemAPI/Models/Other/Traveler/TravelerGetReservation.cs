namespace TicketReservationSystemAPI.Models.Other.Traveler
{
    public class TravelerGetReservation
    {
        public string Id { get; set; }
        public int Seats { get; set; }
        public string BookingDate { get; set; }
        public string ReservationDate { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public bool IsCancelled { get; set; } = false;
    }
}
