// File name: TravelerController.cs
// <summary>
// Description: API controller for traveler related operations
// </summary>
// <author>MulithaBM</author>
// <created>12/09/2023</created>
// <modified>11/10/2023</modified>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Services.TravelerService;

namespace TicketReservationSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelerController : ControllerBase
    {
        private readonly ITravelerService _travelerService;

        public TravelerController(ITravelerService travelerService)
        {
            _travelerService = travelerService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody] TravelerLogin data)
        {
            ServiceResponse<string> response = await _travelerService.Login(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register([FromBody] TravelerRegistration data)
        {
            ServiceResponse<int> response = await _travelerService.Register(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Roles = "Traveler")]
        [HttpGet("account")]
        public async Task<ActionResult<ServiceResponse<Traveler>>> GetAccount()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<Traveler> response = await _travelerService.GetAccount(userId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Roles = "Traveler")]
        [HttpPut("update")]
        public async Task<ActionResult<ServiceResponse<int>>> UpdateAccount([FromBody] TravelerUpdate data)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<int> response = await _travelerService.UpdateAccount(userId, data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Roles = "Traveler")]
        [HttpPut("deactivate")]
        public async Task<ActionResult<ServiceResponse<int>>> DeactivateAccount()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<int> response = await _travelerService.DeactivateAccount(userId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
