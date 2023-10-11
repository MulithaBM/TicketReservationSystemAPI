// File name: AdminTrainService.cs
// <summary>
// Description: Service class for admin train related operations
// </summary>
// <author>MulithaBM</author>
// <created>11/10/2023</created>
// <modified>11/10/2023</modified>

using AutoMapper;
using MongoDB.Driver;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Enums;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public class AdminTrainService : IAdminTrainService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public AdminTrainService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Create a train
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<string>> CreateTrain(AdminCreateTrain data)
        {
            ServiceResponse<string> response = new();

            Train train = new()
            {
                Name = data.Name,
                Type = (TrainType)data.Type,
                Seats = data.Seats,
                DepartureStation = data.DepartureStation.ToLower(),
                ArrivalStation = data.ArrivalStation.ToLower()
            };

            await _context.Trains.InsertOneAsync(train);

            response.Data = train.Id.ToString();
            response.Success = true;
            response.Message = "Train created successfully";

            return response;
        }

        /// <summary>
        /// Get all trains
        /// </summary>
        /// <param name="activeStatus">Active status of the train</param>
        /// <param name="publishStatus">Publish status of the train</param>
        /// <param name="departureStation">Departure station</param>
        /// <param name="arrivalStation">Arrival station</param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<AdminGetTrain>>> GetTrains(
            bool activeStatus, 
            bool publishStatus, 
            string? departureStation, 
            string? arrivalStation)
        {
            ServiceResponse<List<AdminGetTrain>> response = new();

            var filterBuilder = Builders<Train>.Filter;
            var filter = filterBuilder.Empty;

            if (!activeStatus)
            {
                filter &= filterBuilder.Eq(train => train.IsActive, false);
            }
            else
            {
                filter &= filterBuilder.Eq(train => train.IsActive, true);

                if (publishStatus)
                {
                    // Active and Published Trains
                    filter &= filterBuilder.Eq(train => train.IsPublished, true);
                }
                else
                {
                    // Active and Unpublished Trains
                    filter &= filterBuilder.Eq(train => train.IsPublished, false);
                }
            }

            // Apply schedule filters if any of them are present.
            // TODO : Case insensitive search
            if (!string.IsNullOrEmpty(departureStation))
            {
                //departureStation = departureStation.ToLower();
                filter &= filterBuilder.Eq(train => train.DepartureStation, departureStation.ToLower());
            }

            if (!string.IsNullOrEmpty(arrivalStation))
            {
                //arrivalStation = arrivalStation.ToLower();
                filter &= filterBuilder.Eq(train => train.ArrivalStation, arrivalStation.ToLower());
            }

            var filteredTrains = await _context.Trains.Find(filter).ToListAsync();

            response.Data = _mapper.Map<List<AdminGetTrain>>(filteredTrains);
            response.Success = true;

            return response;
        }

        /// <summary>
        /// Get a single train with schedules
        /// </summary>
        /// <param name="id">Train ID</param>
        /// <returns>Train with schedules of type AdminGetTrainWithSchedules</returns>
        public async Task<ServiceResponse<AdminGetTrainWithSchedules>> GetTrain(string id)
        {
            ServiceResponse<AdminGetTrainWithSchedules> response = new();

            Guid trainId = new(id);

            Train train = await _context.Trains.Find(x => x.Id == trainId).FirstOrDefaultAsync();

            if (train == null)
            {
                response.Success = false;
                response.Message = "Train not found";
                return response;
            }

            AdminGetTrainWithSchedules trainWithSchedules = _mapper.Map<AdminGetTrainWithSchedules>(train);

            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            List<TrainSchedule> schedules = await _context.TrainSchedules
                .Find(x => x.TrainId == trainId && x.Date >= today)
                .SortBy(x => x.Date)
                .ThenBy(x => x.DepartureTime)
                .ToListAsync();

            trainWithSchedules.Schedules = _mapper.Map<List<AdminGetTrainSchedule>>(schedules);

            response.Data = trainWithSchedules;
            response.Success = true;

            if (schedules.Count == 0)
            {
                response.Message = "No active schedules found";
            }

            return response;
        }

        // TODO : Implement this
        //public Task<ServiceResponse<string>> UpdateTrain(AdminUpdateTrain data)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<ServiceResponse<bool>> UpdateActiveStatus(string id, bool status)
        {
            ServiceResponse<bool> response = new();

            Guid trainId = new(id);

            Train train = await _context.Trains.Find(x => x.Id == trainId).FirstOrDefaultAsync();

            if (train == null)
            {
                response.Success = false;
                response.Message = "Train not found";
                return response;
            }

            if (!status)
            {
                if (train.ScheduleIDs != null)
                {
                    DateOnly today = DateOnly.FromDateTime(DateTime.Today);

                    List<Guid> schedules = await _context.TrainSchedules
                        .Find(x => x.TrainId == trainId && x.Date >= today)
                        .Project(x => x.Id)
                        .ToListAsync();

                    if (schedules.Count > 0)
                    {
                        // check for reservations in schedules from today onwards
                        bool schedulesWithReservations = schedules.Any(scheduleId =>
                        {
                            TrainSchedule schedule = _context.TrainSchedules.Find(x => x.Id == scheduleId).FirstOrDefault();
                            return schedule != null && schedule.ReservationIDs != null && schedule.ReservationIDs.Any();
                        });

                        if (schedulesWithReservations)
                        {
                            response.Success = false;
                            response.Message = "Cannot deactivate a train with active reservations";
                            return response;
                        }

                        // delete schedules
                        await _context.TrainSchedules.DeleteManyAsync(x => schedules.Contains(x.Id));

                        // remove schedules IDs from train
                        foreach (Guid schedule in schedules)
                        {
                            train.ScheduleIDs.Remove(schedule);
                        }
                    }
                }

                train.IsActive = false;
                train.IsPublished = false;
            }
            else
            {
                train.IsActive = true;
            }

            await _context.Trains.ReplaceOneAsync(x => x.Id == trainId, train);

            response.Data = train.IsActive;
            response.Success = true;
            response.Message = "Train " + (train.IsActive ? "activated " : "deactivated ") + "successfully";

            return response;
        }

        /// <summary>
        /// Update IsPublished property of the train
        /// </summary>
        /// <remarks>
        /// All the active schedules are deleted when train is unpublished
        /// </remarks>
        /// <param name="id">Train ID</param>
        /// <param name="status">Status, true (publish) / false (unpublish)</param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdatePublishStatus(string id, bool status)
        {
            ServiceResponse<bool> response = new();

            Guid trainId = new(id);

            Train train = await _context.Trains.Find(x => x.Id == trainId).FirstOrDefaultAsync();

            if (train == null)
            {
                response.Success = false;
                response.Message = "Train not found";
                return response;
            }

            if (status)
            {
                if (!train.IsActive)
                {
                    response.Success = false;
                    response.Message = "Cannot publish an inactive train";
                    return response;
                }

                train.IsPublished = status;
            }
            else
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.Today);

                List<Guid> schedules = await _context.TrainSchedules
                    .Find(x => x.TrainId == trainId && x.Date >= today)
                    .Project(x => x.Id)
                    .ToListAsync();

                if (schedules.Count > 0)
                {
                    bool schedulesWithReservations = schedules.Any(scheduleId =>
                    {
                        TrainSchedule schedule = _context.TrainSchedules.Find(x => x.Id == scheduleId).FirstOrDefault();
                        return schedule != null && schedule.ReservationIDs.Any();
                    });

                    if (schedulesWithReservations)
                    {
                        response.Success = false;
                        response.Message = "Cannot unpublish train with active reservations";
                        return response;
                    }

                    // Remove all the active schedules
                    await _context.TrainSchedules.DeleteManyAsync(x => schedules.Contains(x.Id));

                    foreach (Guid schedule in schedules)
                    {
                        train.ScheduleIDs.Remove(schedule);
                    }
                }

                train.IsPublished = status;
            }

            await _context.Trains.ReplaceOneAsync(x => x.Id == trainId, train);

            response.Data = train.IsPublished;
            response.Success = true;
            response.Message = "Train " + (train.IsPublished ? "published " : "unpublished ") + "successfully";

            return response;
        }

        /// <summary>
        /// Cancel train for a specific date
        /// </summary>
        /// <remarks>
        /// Can be cancelled only if no reservations are made for the train on the given date
        /// </remarks>
        /// <param name="id">Train ID</param>
        /// <param name="date">Date of cancellation</param>
        /// <returns></returns>
        public async Task<ServiceResponse<string>> CancelTrain(string id, AdminCancelTrain data)
        {
            ServiceResponse<string> response = new();

            Guid trainId = new(id);

            Train train = await _context.Trains.Find(x => x.Id == trainId).FirstOrDefaultAsync();

            if (train == null)
            {
                response.Success = false;
                response.Message = "Train not found";
                return response;
            }

            DateOnly scheduleDate = DateOnly.Parse(data.Date);

            List<Guid> schedules = await _context.TrainSchedules
                .Find(x => x.TrainId == trainId && x.Date == scheduleDate)
                .Project(x => x.Id)
                .ToListAsync();

            if (schedules.Count > 0)
            {
                bool schedulesWithReservations = schedules.Any(scheduleId =>
                {
                    TrainSchedule schedule = _context.TrainSchedules.Find(x => x.Id == scheduleId).FirstOrDefault();
                    return schedule != null && schedule.ReservationIDs.Any();
                });

                if (schedulesWithReservations)
                {
                    response.Success = false;
                    response.Message = "Cannot cancel train with active reservations";
                    return response;
                }

                await _context.TrainSchedules.DeleteManyAsync(x => schedules.Contains(x.Id));

                foreach (Guid schedule in schedules)
                {
                    train.ScheduleIDs.Remove(schedule);
                }
            }

            response.Success = true;
            response.Message = "Train cancelled successfully";

            return response;
        }

        /// <summary>
        /// Add a new schedule to a train
        /// </summary>
        /// <remarks>
        /// Unpublished trains become published automatically when a schedule is added
        /// </remarks>
        /// <param name="data">Schedule data</param>
        /// <returns>ID of the created schedule</returns>
        public async Task<ServiceResponse<string>> AddSchedule(AdminAddSchedule data)
        {
            ServiceResponse<string> response = new();

            Guid trainId = new(data.TrainId);

            Train train = await _context.Trains.Find(x => x.Id == trainId).FirstOrDefaultAsync();

            if (train == null)
            {
                response.Success = false;
                response.Message = "Train not found";
                return response;
            }

            TrainSchedule schedule = new()
            {
                TrainId = trainId,
                Date = DateOnly.Parse(data.Date),
                DepartureTime = TimeOnly.Parse(data.DepartureTime),
                ArrivalTime = TimeOnly.Parse(data.ArrivalTime),
                AvailableSeats = train.Seats,
                Price = decimal.Parse(data.Price)
            };

            await _context.TrainSchedules.InsertOneAsync(schedule);

            train.ScheduleIDs.Add(schedule.Id);

            if (!train.IsPublished)
            {
                train.IsPublished = true;
            }

            await _context.Trains.ReplaceOneAsync(x => x.Id == trainId, train);

            response.Data = schedule.Id.ToString();
            response.Success = true;
            response.Message = "Schedule added successfully";

            return response;
        }

        /// <summary>
        /// Get a single train schedule using schedule ID
        /// </summary>
        /// <param name="id">schedule ID</param>
        /// <returns>Train schedule</returns>
        public async Task<ServiceResponse<AdminGetTrainSchedule>> GetSchedule(string id)
        {
            ServiceResponse<AdminGetTrainSchedule> response = new();

            Guid scheduleId = new(id);

            TrainSchedule schedule = await _context.TrainSchedules.Find(x => x.Id == scheduleId).FirstOrDefaultAsync();

            if (schedule == null)
            {
                response.Success = false;
                response.Message = "Schedule not found";
                return response;
            }

            response.Data = _mapper.Map<AdminGetTrainSchedule>(schedule);
            response.Success = true;

            return response;
        }

        /// <summary>
        /// Update a train schedule
        /// </summary>
        /// <remarks>
        /// Can update only if schedule has no reservations
        /// </remarks>
        /// <param name="id">Schedule ID</param>
        /// <param name="data">Update schedule data</param>
        /// <returns>Train schedule</returns>
        public async Task<ServiceResponse<AdminGetTrainSchedule>> UpdateSchedule(string id, AdminUpdateSchedule data)
        {
            ServiceResponse<AdminGetTrainSchedule> response = new();

            Guid scheduleId = new(id);

            TrainSchedule schedule = await _context.TrainSchedules.Find(x => x.Id == scheduleId).FirstOrDefaultAsync();

            if (schedule == null)
            {
                response.Success = false;
                response.Message = "Schedule not found";
                return response;
            }

            // check if the schedule has reservations
            if (schedule.ReservationIDs != null && schedule.ReservationIDs.Any())
            {
                response.Success = false;
                response.Message = "Cannot update schedule with active reservations";
                return response;
            }

            TimeOnly departureTime = (data.DepartureTime != null) ? TimeOnly.Parse(data.DepartureTime) : schedule.DepartureTime;
            TimeOnly arrivalTime = (data.ArrivalTime != null) ? TimeOnly.Parse(data.ArrivalTime) : schedule.ArrivalTime;

            if (data.DepartureTime != null) schedule.DepartureTime = TimeOnly.Parse(data.DepartureTime);
            if (data.ArrivalTime != null) schedule.ArrivalTime = TimeOnly.Parse(data.ArrivalTime);
            if (data.AvailableSeats != null) schedule.AvailableSeats = (int)data.AvailableSeats;
            if (data.Price != null) schedule.Price = decimal.Parse(data.Price);

            await _context.TrainSchedules.ReplaceOneAsync(x => x.Id == scheduleId, schedule);

            response.Data = _mapper.Map<AdminGetTrainSchedule>(schedule);
            response.Success = true;
            response.Message = "Schedule updated successfully";

            return response;
        }

        /// <summary>
        /// Delete a train schedule
        /// </summary>
        /// <remarks>
        /// Can delete only if schedule has no reservations
        /// If no active schedules are present, train is unpublished automatically
        /// </remarks>
        /// <param name="id">Train ID</param>
        /// <returns></returns>
        public async Task<ServiceResponse<string>> DeleteSchedule(string id)
        {
            ServiceResponse<string> response = new();

            Guid scheduleId = Guid.Parse(id);

            TrainSchedule schedule = await _context.TrainSchedules.Find(x => x.Id == scheduleId).FirstOrDefaultAsync();

            if (schedule == null)
            {
                response.Success = false;
                response.Message = "Schedule not found";
                return response;
            }

            // check if the schedule has reservations
            if (schedule.ReservationIDs.Any())
            {
                response.Success = false;
                response.Message = "Cannot delete schedule with active reservations";
                return response;
            }

            await _context.TrainSchedules.DeleteOneAsync(x => x.Id == scheduleId);

            Train train = await _context.Trains.Find(x => x.Id == schedule.TrainId).FirstOrDefaultAsync();

            if (train != null)
            {
                train.ScheduleIDs.Remove(scheduleId);

                DateTime current = DateTime.Now;
                DateOnly currentDate = DateOnly.FromDateTime(current);
                TimeOnly currentTime = TimeOnly.FromDateTime(current);

                List<TrainSchedule> activeSchedules = await _context.TrainSchedules
                    .Find(s => s.TrainId == train.Id && s.Date >= currentDate && s.DepartureTime >= currentTime)
                    .ToListAsync();

                if (activeSchedules.Count == 0)
                {
                    train.IsPublished = false;
                }

                await _context.Trains.ReplaceOneAsync(x => x.Id == train.Id, train);
            }

            response.Success = true;
            response.Message = "Schedule deleted successfully";
            return response;
        }

        //private async Task<bool> HasActiveReservations(Guid trainId, List<TrainSchedule> schedules)
        //{
        //    if (schedules.Count > 0)
        //    {
        //        foreach (TrainSchedule schedule in schedules)
        //        {
        //            if (schedule.Reservations != null && schedule.Reservations.Any())
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}
        //private async Task<bool> HasActiveReservations(Guid trainId, List<TrainSchedule> schedules, DateOnly date)
        //{

        //    List<TrainSchedule> schedules = await _context.TrainSchedules.Find(x => x.TrainId == trainId && x.Date >= today).ToListAsync();

        //    if (schedules.Count > 0)
        //    {
        //        foreach (TrainSchedule schedule in schedules)
        //        {
        //            if (schedule.Reservations != null && schedule.Reservations.Any())
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}
    }
}
