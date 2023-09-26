using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.TravelerService
{
    public interface ITravelerService
    {
        Task<ServiceResponse<Traveler>> GetProfile(string userId);
    }
}
