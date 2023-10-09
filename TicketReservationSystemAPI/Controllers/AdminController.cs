using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;
using TicketReservationSystemAPI.Services.AdminService;

namespace TicketReservationSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAdminTravelerService _adminTravelerService;

        public AdminController(IAdminService adminService, IAdminTravelerService adminTravelerService)
        {
            _adminService = adminService;
            _adminTravelerService = adminTravelerService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody] AdminLogin data)
        {
            ServiceResponse<string> response = await _adminService.Login(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<string>>> Register([FromBody] AdminRegistration data)
        {
            ServiceResponse<string> response = await _adminService.Register(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("account")]
        public async Task<ActionResult<ServiceResponse<AdminReturn>>> GetAccount()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<AdminReturn> response = await _adminService.GetAccount(userId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("account")]
        public async Task<ActionResult<ServiceResponse<AdminReturn>>> UpdateAccount([FromBody] AdminUpdate data)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<AdminReturn> response = await _adminService.UpdateAccount(userId, data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("account")]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteAccount()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<string> response = await _adminService.DeleteAccount(userId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("traveler/account")]
        public async Task<ActionResult<ServiceResponse<string>>> CreateTravelAccount([FromBody] AdminTravelerRegistration data)
        {
            ServiceResponse<string> response = await _adminTravelerService.CreateAccount(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("traveler/accounts")]
        public async Task<ActionResult<ServiceResponse<List<AdminTravelerReturn>>>> GetTravelerAccounts()
        {
            ServiceResponse<List<AdminTravelerReturn>> response = await _adminTravelerService.GetAccounts();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("traveler/account/{nic}")]
        public async Task<ActionResult<ServiceResponse<AdminTravelerReturn>>> GetTravelerAccount(string nic)
        {
            ServiceResponse<AdminTravelerReturn> response = await _adminTravelerService.GetAccount(nic);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("traveler/account/{nic}")]
        public async Task<ActionResult<ServiceResponse<AdminTravelerReturn>>> UpdateTravelerAccount(string nic, [FromBody] AdminTravelerUpdate data)
        {
            ServiceResponse<AdminTravelerReturn> response = await _adminTravelerService.UpdateAccount(nic, data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("traveler/account/{nic}/active")]
        public async Task<ActionResult<ServiceResponse<AdminTravelerReturn>>> UpdateTravelerActiveStatus(string nic)
        {
            ServiceResponse<AdminTravelerReturn> response = await _adminTravelerService.UpdateActiveStatus(nic);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("traveler/account/{nic}")]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteTravelerAccount(string nic)
        {
            ServiceResponse<string> response = await _adminTravelerService.DeleteAccount(nic);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
