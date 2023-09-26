using Microsoft.AspNetCore.Mvc;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Services.AuthService;

namespace TicketReservationSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentController : ControllerBase
    {
        private readonly IAgentAuthService _authService;
        public AgentController(IAgentAuthService authService) 
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody] AgentLogin data)
        {
            ServiceResponse<string> response = await _authService.Login(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register([FromBody] AgentRegistration data)
        {
            ServiceResponse<int> response = await _authService.Register(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
