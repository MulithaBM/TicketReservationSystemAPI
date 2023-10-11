// File name: ServiceResponse.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>12/09/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }
}
