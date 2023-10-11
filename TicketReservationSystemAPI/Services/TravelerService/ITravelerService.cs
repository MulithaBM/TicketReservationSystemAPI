// File name: ITravelerService.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>23/09/2023</created>
// <modified>11/10/2023</modified>

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
