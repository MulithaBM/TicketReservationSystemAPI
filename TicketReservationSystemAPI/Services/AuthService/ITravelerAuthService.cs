using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.AuthService
{
    public interface ITravelerAuthService
    {
        Task<ServiceResponse<int>> Register(TravelerRegistration traveler);
        Task<ServiceResponse<string>> Login(TravelerLogin traveler);
        Task<bool> UserExists(string email);
    }
}
