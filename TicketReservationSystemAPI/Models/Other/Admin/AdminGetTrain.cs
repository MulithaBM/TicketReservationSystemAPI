// File name: AdminGetTrain.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminGetTrain
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsPublished { get; set; } = false;
    }
}
