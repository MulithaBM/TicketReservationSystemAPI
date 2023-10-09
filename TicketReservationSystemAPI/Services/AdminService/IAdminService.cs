using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public interface IAdminService
    {
        Task<ServiceResponse<string>> Register(AdminRegistration data);
        Task<ServiceResponse<string>> Login(AdminLogin data);
        Task<ServiceResponse<AdminReturn>> GetAccount(string userId);
        Task<ServiceResponse<AdminReturn>> UpdateAccount(string userId, AdminUpdate data);
        Task<ServiceResponse<string>> DeleteAccount(string userId);
    }
}
