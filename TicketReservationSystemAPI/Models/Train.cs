﻿// File name: Train.cs
// <summary>
// Description: Model class for trains.
// </summary>
// <author> MulithaBM </author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

using TicketReservationSystemAPI.Models.Enums;

namespace TicketReservationSystemAPI.Models
{
    public class Train
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TrainType Type { get; set; }
        public int Seats { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public IList<Guid> ScheduleIDs { get; set; } = new List<Guid>();
        public IList<DateOnly> AvailableDates { get; set; } = new List<DateOnly>();
        public bool IsActive { get; set; } = true;
        public bool IsPublished { get; set; } = false;
    }
}
