namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminCreateReservation
    {
        public string NIC { get; set; }
        public string ScheduleId { get; set; }
        public int Seats { get; set; }
    }
}
