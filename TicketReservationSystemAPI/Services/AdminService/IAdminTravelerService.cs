using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public interface IAdminTravelerService
    {
        Task<ServiceResponse<string>> CreateAccount(AdminTravelerRegistration data);
        Task<ServiceResponse<AdminTravelerReturn>> GetAccount(string userId);
        Task<ServiceResponse<List<AdminTravelerReturn>>> GetAccounts();
        Task<ServiceResponse<AdminTravelerReturn>> UpdateAccount(string userId, AdminTravelerUpdate data);
        Task<ServiceResponse<AdminTravelerReturn>> UpdateActiveStatus(string userId);
        Task<ServiceResponse<string>> DeleteAccount(string userId);
    }
}
