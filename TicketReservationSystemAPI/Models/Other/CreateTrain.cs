using MongoDB.Bson;

namespace TicketReservationSystemAPI.Models.Other
{
    public class CreateTrain
    {
        public string Name { get; set; }
        public int Seats { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
    }
}
