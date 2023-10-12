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
using TicketReservationSystemAPI.Models.Other.Traveler;
using TicketReservationSystemAPI.Services.TravelerService;

namespace TicketReservationSystemAPI.Controllers
{
    [Authorize(Roles = "Traveler")]
    [ApiController]
    [Route("api/[controller]")]
    public class TravelerController : ControllerBase
    {
        private readonly ITravelerService _travelerService;
        private readonly ITravelerTrainService _travelerTrainService;
        private readonly ITravelerReservationService _travelerReservationService;

        public TravelerController(
            ITravelerService travelerService, 
            ITravelerTrainService travelerTrainService, 
            ITravelerReservationService travelerReservationService)
        {
            _travelerService = travelerService;
            _travelerTrainService = travelerTrainService;
            _travelerReservationService = travelerReservationService;
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<string>>> Register([FromBody] TravelerRegistration data)
        {
            ServiceResponse<string> response = await _travelerService.Register(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("account")]
        public async Task<ActionResult<ServiceResponse<TravelerReturn>>> GetAccount()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<TravelerReturn> response = await _travelerService.GetAccount(userId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("update")]
        public async Task<ActionResult<ServiceResponse<string>>> UpdateAccount([FromBody] TravelerUpdate data)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<string> response = await _travelerService.UpdateAccount(userId, data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("deactivate")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeactivateAccount()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<bool> response = await _travelerService.DeactivateAccount(userId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // Train endpoints

        [HttpGet("trains")]
        public async Task<ActionResult<ServiceResponse<List<TravelerGetTrain>>>> GetTrains(
            [FromQuery] string? departureStation, 
            [FromQuery] string? arrivalStation, 
            [FromQuery] string? date)
        {
            ServiceResponse<List<TravelerGetTrain>> response = await _travelerTrainService.GetTrains(departureStation, arrivalStation, date);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("train/{id}")]
        public async Task<ActionResult<ServiceResponse<TravelerGetTrainWithSchedules>>> GetTrain(string id)
        {
            ServiceResponse<TravelerGetTrainWithSchedules> response = await _travelerTrainService.GetTrain(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("train/schedule/{id}")]
        public async Task<ActionResult<ServiceResponse<TravelerGetTrainSchedule>>> GetSchedule(string id)
        {
            ServiceResponse<TravelerGetTrainSchedule> response = await _travelerTrainService.GetSchedule(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // Reservation endpoints

        [HttpPost("reservation")]
        public async Task<ActionResult<ServiceResponse<string>>> CreateReservation([FromBody] TravelerCreateReservation data)
        {
            string nic = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<string> response = await _travelerReservationService.CreateReservation(nic, data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("reservations")]
        public async Task<ActionResult<ServiceResponse<List<TravelerGetReservation>>>> GetReservations([FromQuery] bool past = false)
        {
            string nic = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<List<TravelerGetReservation>> response = await _travelerReservationService.GetReservations(nic, past);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("reservation/{id}")]
        public async Task<ActionResult<ServiceResponse<TravelerGetReservation>>> GetReservation(string id)
        {
            ServiceResponse<TravelerGetReservation> response = await _travelerReservationService.GetReservation(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("reservation/{id}")]
        public async Task<ActionResult<ServiceResponse<TravelerGetReservation>>> UpdateReservation(
            string id,
            [FromBody] TravelerUpdateReservation data)
        {
            ServiceResponse<TravelerGetReservation> response = await _travelerReservationService.UpdateReservation(id, data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("reservation/{id}/cancel")]
        public async Task<ActionResult<ServiceResponse<string>>> CancelReservation(string id)
        {
            ServiceResponse<string> response = await _travelerReservationService.CancelReservation(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
