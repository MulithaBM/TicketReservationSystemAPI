namespace TicketReservationSystemAPI.Models.Other
{
    public class CreateTrainSchedule
    {
        public DateOnly Date { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public decimal Price { get; set; }
    }
}
