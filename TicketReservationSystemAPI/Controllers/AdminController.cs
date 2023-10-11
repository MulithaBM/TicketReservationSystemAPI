// File name: AdminController.cs
// <summary>
// Description: API controller for back-office related operations
// </summary>
// <author>MulithaBM</author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;
using TicketReservationSystemAPI.Services.AdminService;

namespace TicketReservationSystemAPI.Controllers
{
    /// <permission cref="ClaimTypes.Role">
    /// Admin (Back-Office) has access to the following endpoints:
    /// </permission>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAdminTravelerService _adminTravelerService;
        private readonly IAdminTrainService _adminTrainService;

        public AdminController(IAdminService adminService, IAdminTravelerService adminTravelerService, IAdminTrainService adminTrainService)
        {
            _adminService = adminService;
            _adminTravelerService = adminTravelerService;
            _adminTrainService = adminTrainService;
        }

        /// <summary>
        /// Back-Office login
        /// </summary>
        /// <Permission>
        /// Anyone can access this endpoint
        /// </Permission>
        /// <param name="data">Login data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> object with the token as data
        /// </returns>
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

        /// <summary>
        /// Back-Office registration
        /// </summary>
        /// <Permission>
        /// Anyone can access this endpoint
        /// </Permission>
        /// <param name="data">Registration data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> object with null as data
        /// </returns>
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

        /// <summary>
        /// Get current Back-Office account
        /// </summary>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> object with the account as data
        /// </returns>
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

        /// <summary>
        /// Update current Back-Office account
        /// </summary>
        /// <param name="data">Update data></param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> object with the updated account as data
        /// </returns>
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

        /// <summary>
        /// Delete current Back-Office account
        /// </summary>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> object with null as data
        /// </returns>
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
        public async Task<ActionResult<ServiceResponse<List<AdminTravelerReturn>>>> GetTravelerAccounts([FromQuery] bool? status)
        {
            ServiceResponse<List<AdminTravelerReturn>> response = await _adminTravelerService.GetAccounts(status);

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

        [HttpPost("train")]
        public async Task<ActionResult<ServiceResponse<string>>> CreateTrain([FromBody] AdminCreateTrain data)
        {
            ServiceResponse<string> response = await _adminTrainService.CreateTrain(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("trains")]
        public async Task<ActionResult<ServiceResponse<List<AdminGetTrain>>>> GetTrains(
            [FromQuery] bool activeStatus = true,
            [FromQuery] bool publishStatus = true,
            [FromQuery] string? departureStation = null,
            [FromQuery] string? arrivalStation = null)
        {
            ServiceResponse<List<AdminGetTrain>> response = await _adminTrainService.GetTrains(activeStatus, publishStatus, departureStation, arrivalStation);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("train/{id}")]
        public async Task<ActionResult<ServiceResponse<AdminGetTrainWithSchedules>>> GetTrain(string id)
        {
            ServiceResponse<AdminGetTrainWithSchedules> response = await _adminTrainService.GetTrain(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        //[HttpPut("train/{id}")]
        //public async Task<ActionResult<ServiceResponse<string>>> UpdateTrain(
        //    string id,
        //    [FromBody] AdminUpdateTrain data)
        //{
        //    ServiceResponse<string> response = await _adminTrainService.UpdateTrain(data);

        //    if (!response.Success)
        //    {
        //        return BadRequest(response);
        //    }

        //    return Ok(response);
        //}

        [HttpPut("train/{id}/active")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateTrainActiveStatus(
            string id,
            [FromBody] bool status)
        {
            ServiceResponse<bool> response = await _adminTrainService.UpdateActiveStatus(id, status);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("train/{id}/publish")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateTrainPublishStatus(
            string id,
            [FromBody] bool status)
        {
            ServiceResponse<bool> response = await _adminTrainService.UpdatePublishStatus(id, status);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("train/schedule")]
        public async Task<ActionResult<ServiceResponse<string>>> AddSchedule([FromBody] AdminAddSchedule data)
        {
            ServiceResponse<string> response = await _adminTrainService.AddSchedule(data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("train/schedule/{id}")]
        public async Task<ActionResult<ServiceResponse<AdminGetTrainSchedule>>> GetSchedule(string id)
        {
            ServiceResponse<AdminGetTrainSchedule> response = await _adminTrainService.GetSchedule(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("train/schedule/{id}")]
        public async Task<ActionResult<ServiceResponse<AdminGetTrainSchedule>>> UpdateSchedule(
            string id,
            [FromBody] AdminUpdateSchedule data)
        {
            ServiceResponse<AdminGetTrainSchedule> response = await _adminTrainService.UpdateSchedule(id, data);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("train/schedule/{id}")]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteSchedule(string id)
        {
            ServiceResponse<string> response = await _adminTrainService.DeleteSchedule(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("train/schedule/{id}/cancel")]
        public async Task<ActionResult<ServiceResponse<string>>> CancelSchedule(
            string id,
            [FromBody] string date)
        {
            ServiceResponse<string> response = await _adminTrainService.CancelTrain(id, date);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
