namespace TicketReservationSystemAPI.Models.Other.Traveler
{
    public class TravelerGetTrainSchedule
    {
        public string Id { get; set; }
        public string Date { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }
    }
}
