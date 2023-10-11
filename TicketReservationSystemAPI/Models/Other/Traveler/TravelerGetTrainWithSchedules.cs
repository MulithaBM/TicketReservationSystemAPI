namespace TicketReservationSystemAPI.Models.Other.Traveler
{
    public class TravelerGetTrainWithSchedules
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Seats { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public IList<TravelerGetTrainSchedule> Schedules { get; set; } = new List<TravelerGetTrainSchedule>();
    }
}
