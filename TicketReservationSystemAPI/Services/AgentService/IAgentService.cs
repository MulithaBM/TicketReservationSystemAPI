using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.AgentService
{
    public interface IAgentService
    {
        Task<ServiceResponse<int>> Register(AgentRegistration traveler);
        Task<ServiceResponse<string>> Login(AgentLogin traveler);
    }
}
