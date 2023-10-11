﻿using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public interface IAdminTrainService
    {
        Task<ServiceResponse<string>> CreateTrain(AdminCreateTrain data);
        Task<ServiceResponse<List<AdminGetTrain>>> GetTrains(bool activeStatus = true, bool publishStatus = true, string? departureStation = null, string? arrivalStation = null);
        Task<ServiceResponse<AdminGetTrainWithSchedules>> GetTrain(string id);
        // Task<ServiceResponse<string>> UpdateTrain(AdminUpdateTrain data);
        Task<ServiceResponse<bool>> UpdateActiveStatus(string id, bool status);
        Task<ServiceResponse<bool>> UpdatePublishStatus(string id, bool status);    
        Task<ServiceResponse<string>> CancelTrain(string id, string date);
        Task<ServiceResponse<string>> AddSchedule(AdminAddSchedule data);
        Task<ServiceResponse<AdminGetTrainSchedule>> GetSchedule(string id);
        Task<ServiceResponse<AdminGetTrainSchedule>> UpdateSchedule(string id, AdminUpdateSchedule data);
        Task<ServiceResponse<string>> DeleteSchedule(string id);
    }
}
