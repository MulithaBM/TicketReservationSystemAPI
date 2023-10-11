﻿using AutoMapper;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Admin, AdminReturn>();
            CreateMap<Traveler, AdminTravelerReturn>();
            CreateMap<Train, AdminGetTrain>();
            CreateMap<Train, AdminGetTrainWithSchedules>();
            CreateMap<TrainSchedule, AdminGetTrainSchedule>();
        }
    }
}
