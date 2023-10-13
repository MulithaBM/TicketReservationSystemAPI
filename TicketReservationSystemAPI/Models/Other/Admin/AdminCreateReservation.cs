// File name: AdminCreateReservation.cs
// <summary>
// Description: Data transfer model to create reservation, for admin.
// </summary>
// <author> MulithaBM </author>
// <created>11/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminCreateReservation
    {
        public string NIC { get; set; }
        public string ScheduleId { get; set; }
        public int Seats { get; set; }
    }
}
