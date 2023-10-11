using TicketReservationSystemAPI.Models.Enums;

namespace TicketReservationSystemAPI.Models.Other.Traveler
{
    public class TravelerGetTrain
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public String Type { get; set; }
        public int Seats { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
    }
}
