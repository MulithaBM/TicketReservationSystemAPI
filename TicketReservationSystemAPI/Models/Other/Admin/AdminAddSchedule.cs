// File name: AdminAddSchedule.cs
// <summary>
// Description: Data transfer model to add schedule, for admin.
// </summary>
// <author> MulithaBM </author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminAddSchedule
    {
        public string TrainId { get; set; }
        public string Date { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string Price { get; set; }
    }
}
