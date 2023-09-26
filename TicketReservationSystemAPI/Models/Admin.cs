using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketReservationSystemAPI.Models
{
    public class Admin
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
