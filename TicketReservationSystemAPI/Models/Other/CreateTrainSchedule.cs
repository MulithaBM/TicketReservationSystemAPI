﻿// File name: CreateTrainSchedule.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>30/09/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other
{
    public class CreateTrainSchedule
    {
        public DateOnly Date { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public decimal Price { get; set; }
    }
}
