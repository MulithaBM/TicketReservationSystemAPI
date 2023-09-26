using System.Security.Claims;
using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Middlewares
{
    public class UserIdMiddleware
    {
        private readonly RequestDelegate _next;

        public UserIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            ServiceResponse<string> response = new();

            if (context.User.Identity is ClaimsIdentity claimsIdentity)
            {
                var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    context.Items["UserId"] = userIdClaim.Value;
                }
                else
                {
                    response.Success = false;
                    response.Message = "User not found";
                    
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsJsonAsync(response);
                    return ;
                }
            }

            await _next(context);
        }
    }
}
