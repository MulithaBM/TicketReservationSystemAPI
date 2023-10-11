// File name: AdminTravelerReturn.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

using MongoDB.Bson;

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminGetTraveler
    {
        public string NIC { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public bool IsActive { get; set; }
    }
}
