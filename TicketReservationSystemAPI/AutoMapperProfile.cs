// File name: AutoMapperProfile.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>11/10/2023</created>
// <modified>11/10/2023</modified>

using AutoMapper;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other.Admin;
using TicketReservationSystemAPI.Models.Other.Traveler;

namespace TicketReservationSystemAPI
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Admin
            CreateMap<Admin, AdminReturn>();
            CreateMap<Traveler, AdminGetTraveler>();
            CreateMap<Train, AdminGetTrain>();
            CreateMap<Train, AdminGetTrainWithSchedules>();
            CreateMap<TrainSchedule, AdminGetTrainSchedule>();
            CreateMap<Traveler, AdminGetTravelerWithReservations>();
            CreateMap<Reservation, AdminGetReservation>();

            // Traveler
            CreateMap<Reservation, TravelerGetReservation>();
            CreateMap<Traveler, TravelerReturn>();
            CreateMap<Train, TravelerGetTrain>();
            CreateMap<Train, TravelerGetTrainWithSchedules>();
            CreateMap<TrainSchedule, TravelerGetTrainSchedule>();
        }
    }
}
