// File name: Admin.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>11/09/2023</created>
// <modified>11/10/2023</modified>

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketReservationSystemAPI.Models
{
    public class Admin
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
