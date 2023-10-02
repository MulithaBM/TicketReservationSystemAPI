using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.TrainService
{
    public interface ITrainService
    {
        Task<ServiceResponse<IEnumerable<Train>>> GetTrains();
        Task<ServiceResponse<IEnumerable<Train>>> GetTrainS(bool isActive, bool isPublished, string departureStation, string arrivalStation, string date);
        Task<ServiceResponse<Train>> GetTrain(string trainId);
        Task<ServiceResponse<int>> CreateTrain(CreateTrain train);
        Task<ServiceResponse<int>> UpdateTrain(string trainId, UpdateTrain train);
        Task<ServiceResponse<int>> DeleteTrain(string trainId);
        Task<ServiceResponse<int>> AddTrainSchedule(CreateTrainSchedule trainSchedule);
        Task<ServiceResponse<int>> UpdateTrainSchedule(string scheduleId, UpdateTrainSchedule trainSchedule);
        Task<ServiceResponse<int>> DeleteTrainSchedule(string scheduleId);
    }
}
