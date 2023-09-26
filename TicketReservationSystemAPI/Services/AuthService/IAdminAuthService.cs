using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.AuthService
{
    public interface IAdminAuthService
    {
        //Task<ServiceResponse<int>> Register(BackOfficeRegistration traveler);
        Task<ServiceResponse<string>> Login(AdminLogin traveler);
        Task<bool> UserExists(string email);
    }
}
