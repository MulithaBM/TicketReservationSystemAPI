// File name: IAdminReservationService.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>11/10/2023</created>
// <modified>11/10/2023</modified>

using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public interface IAdminReservationService
    {
        Task<ServiceResponse<string>> CreateReservation(AdminCreateReservation data);
        //Task<ServiceResponse<List<AdminGetReservation>>> GetReservations(string? userId = null, string? trainId = null, string? date = null);
        Task<ServiceResponse<AdminGetReservation>> GetReservation(string id);
        Task<ServiceResponse<AdminGetReservation>> UpdateReservation(string id, AdminUpdateReservation data);
        Task<ServiceResponse<string>> CancelReservation(string id);
    }
}
