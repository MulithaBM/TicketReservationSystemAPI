using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Data
{
    public interface IAuthRepository<T>
    {
        Task<ServiceResponse<int>> Register(T user);
        Task<ServiceResponse<string>> Login(T user);
        Task<bool> UserExists(string email);
    }
}
