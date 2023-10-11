// File name: TrainController.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Services.TrainService;

namespace TicketReservationSystemAPI.Controllers
{
    public class TrainController : ControllerBase
    {
        private readonly ITrainService _trainService;

        public TrainController(ITrainService trainService)
        {
            _trainService = trainService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<IEnumerable<Train>>>> GetTrains(
            [FromQuery] bool isActive,
            [FromQuery] bool isPublished,
            [FromQuery] string arrivalStation,
            [FromQuery] string departureStation,
            [FromQuery] string Date)
        {
            ServiceResponse<IEnumerable<Train>> response = new();

            if (HttpContext.Request.Query.Count == 0)
            {
                response = await _trainService.GetTrains();
            }
            else
            {
                response = await _trainService.GetTrainS(isActive, isPublished, arrivalStation, departureStation, Date);
            }

            if (response.Success == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<Train>>> GetTrain(string id)
        {
            ServiceResponse<Train> response = await _trainService.GetTrain(id);

            if (response.Success == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<int>>> CreateTrain([FromBody] CreateTrain train)
        {
            ServiceResponse<int> response = await _trainService.CreateTrain(train);

            if (response.Success == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> AddTrainSchedule([FromBody] CreateTrainSchedule trainSchedule)
        {
            ServiceResponse<int> response = await _trainService.AddTrainSchedule(trainSchedule);

            if (response.Success == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
