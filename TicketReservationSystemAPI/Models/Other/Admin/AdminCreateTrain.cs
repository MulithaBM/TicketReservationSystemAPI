﻿// File name: AdminCreateTrain.cs
// <summary>
// Description: Data transfer model to create train, for admin.
// </summary>
// <author> MulithaBM </author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminCreateTrain
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public int Seats { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
    }
}
