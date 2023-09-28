using Microsoft.AspNetCore.Mvc;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Services.AgentService;

namespace TicketReservationSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _agentService;
        public AgentController(IAgentService authService) 
        {
            _agentService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody] AgentLogin data)
        {
            ServiceResponse<string> response = await _agentService.Login(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register([FromBody] AgentRegistration data)
        {
            ServiceResponse<int> response = await _agentService.Register(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
