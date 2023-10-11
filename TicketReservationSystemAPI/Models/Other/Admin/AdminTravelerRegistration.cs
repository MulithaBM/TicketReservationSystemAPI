// File name: AdminTravelerRegistration.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminTravelerRegistration
    {
        public string NIC { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
