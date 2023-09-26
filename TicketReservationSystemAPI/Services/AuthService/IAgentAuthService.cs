using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.AuthService
{
    public interface IAgentAuthService
    {
        Task<ServiceResponse<int>> Register(AgentRegistration traveler);
        Task<ServiceResponse<string>> Login(AgentLogin traveler);
        Task<bool> UserExists(string email);
    }
}
