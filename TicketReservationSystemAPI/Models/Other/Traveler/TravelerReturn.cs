namespace TicketReservationSystemAPI.Models.Other.Traveler
{
    public class TravelerReturn
    {
        public string NIC { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public bool IsActive { get; set; }
        public IList<TravelerGetReservation> Reservations { get; set; } = new List<TravelerGetReservation>();
    }
}
