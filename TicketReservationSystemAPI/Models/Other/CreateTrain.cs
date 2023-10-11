// File name: CreateTrain.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

using MongoDB.Bson;

namespace TicketReservationSystemAPI.Models.Other
{
    public class CreateTrain
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public int Seats { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
    }
}
