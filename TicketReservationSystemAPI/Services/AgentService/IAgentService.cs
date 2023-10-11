// File name: IAgentService.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>28/09/2023</created>
// <modified>11/10/2023</modified>

using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.AgentService
{
    public interface IAgentService
    {
        Task<ServiceResponse<int>> Register(AgentRegistration traveler);
        Task<ServiceResponse<string>> Login(AgentLogin traveler);
    }
}
