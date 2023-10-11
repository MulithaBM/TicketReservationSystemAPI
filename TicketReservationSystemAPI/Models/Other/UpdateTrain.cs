// File name: UpdateTrain.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>30/09/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other
{
    public class UpdateTrain
    {
        public string Name { get; set; }
        public int Seats { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
    }
}
