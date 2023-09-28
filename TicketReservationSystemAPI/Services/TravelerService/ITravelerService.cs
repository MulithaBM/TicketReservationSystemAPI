using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.TravelerService
{
    public interface ITravelerService
    {
        Task<ServiceResponse<int>> Register(TravelerRegistration traveler);
        Task<ServiceResponse<string>> Login(TravelerLogin traveler);
        Task<ServiceResponse<Traveler>> GetAccount(string userId);
        Task<ServiceResponse<int>> UpdateAccount(string userId, TravelerUpdate traveler);
        Task<ServiceResponse<int>> DeactivateAccount(string userId);
    }
}
