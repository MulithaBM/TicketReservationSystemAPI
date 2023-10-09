using MongoDB.Bson;

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminTravelerReturn
    {
        public string NIC { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public bool IsActive { get; set; }
    }
}
