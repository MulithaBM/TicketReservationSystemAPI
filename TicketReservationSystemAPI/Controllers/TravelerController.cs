using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.enums;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Services.AuthService;
using TicketReservationSystemAPI.Services.TravelerService;

namespace TicketReservationSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelerController : ControllerBase
    {
        private readonly ITravelerAuthService _authService;
        private readonly ITravelerService _travelerService;

        public TravelerController(ITravelerAuthService authService, ITravelerService travelerService)
        {
            _authService = authService;
            _travelerService = travelerService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody] TravelerLogin data)
        {
            ServiceResponse<string> response = await _authService.Login(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register([FromBody] TravelerRegistration data)
        {
            ServiceResponse<int> response = await _authService.Register(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Roles = "Traveler")]
        [HttpGet("profile")]
        public async Task<ActionResult<ServiceResponse<Traveler>>> GetProfile()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<Traveler> response = await _travelerService.GetProfile(userId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
