namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminGetTravelerWithReservations
    {
        public string NIC { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public List<AdminGetReservation> Reservations { get; set; } = new();
        public bool IsActive { get; set; }
    }
}
