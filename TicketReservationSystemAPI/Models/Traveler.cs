using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using TicketReservationSystemAPI.Models.enums;

namespace TicketReservationSystemAPI.Models
{
    public class Traveler
    {
        [BsonId]
        public string NIC { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsActive { get; set; }
    }
}
