using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Enums;
using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.TrainService
{
    public class AdminTrainService : IAdminTrainService
    {
        private readonly DataContext _context;

        public AdminTrainService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<int>> CreateTrain(CreateTrain data)
        {
            ServiceResponse<int> response = new();

            try
            {
                Train train = new()
                {
                    Name = data.Name,
                    Type = (TrainType)data.Type,
                    DepartureStation = data.DepartureStation,
                    ArrivalStation = data.ArrivalStation
                };

                await _context.Trains.InsertOneAsync(train);

                response.Success = true;
                response.Message = "Train created.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<Train>> GetTrain(string id)
        {
            ServiceResponse<Train> response = new();

            try
            {
                Train train = await _context.Trains.Find(x => x.Id == new ObjectId(id)).FirstOrDefaultAsync();

                if (train == null)
                {
                    response.Success = false;
                    response.Message = "Train not found.";
                    return response;
                }

                response.Data = train;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<Train>>> GetTrains()
        {
            ServiceResponse<IEnumerable<Train>> response = new();

            try
            {
                IEnumerable<Train> trains = await _context.Trains.Find(x => true).ToListAsync();

                response.Data = trains;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<Train>>> GetTrainS(
            bool isActive,
            bool isPublished,
            string departureStation, 
            string arrivalStation, 
            string date)
        {
            ServiceResponse<IEnumerable<Train>> response = new ServiceResponse<IEnumerable<Train>>();

            try
            {
                var filterBuilder = Builders<Train>.Filter;
                var filter = filterBuilder.Empty;

                if (!isActive)
                {
                    filter = filterBuilder.Eq(train => train.IsActive, false);
                }
                else
                {
                    var isActiveFilter = filterBuilder.Eq(train => train.IsActive, true);

                    if (isPublished)
                    {
                        // Active and Published Trains
                        var isPublishedFilter = filterBuilder.Eq(train => train.IsPublished, true);
                        filter = filterBuilder.And(isActiveFilter, isPublishedFilter);
                    }
                    else
                    {
                        // Active and Unpublished Trains
                        var isUnpublishedFilter = filterBuilder.Eq(train => train.IsPublished, false);
                        filter = filterBuilder.And(isActiveFilter, isUnpublishedFilter);
                    }

                    // Apply schedule filters if any of them are present.
                    if (!string.IsNullOrEmpty(departureStation))
                    {
                        filter = filterBuilder.And(filter, filterBuilder.Eq(train => train.DepartureStation, departureStation));
                    }

                    if (!string.IsNullOrEmpty(arrivalStation))
                    {
                        filter = filterBuilder.And(filter, filterBuilder.Eq(train => train.ArrivalStation, arrivalStation));
                    }

                    if (!date.IsNullOrEmpty())
                    {
                        DateOnly? scheduleDate = DateOnly.FromDateTime(DateTime.Parse(date));

                        var scheduleIds = await _context.TrainSchedules.Find(schedule => schedule.Date == scheduleDate).Project(schedule => schedule.Id).ToListAsync();

                        filter = filterBuilder.And(filter, filterBuilder.AnyIn(train => train.Schedules, scheduleIds));
                    }
                }

                var filteredTrains = await _context.Trains.Find(filter).ToListAsync();
                response.Data = filteredTrains;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<int>> UpdateTrain(string id, UpdateTrain data)
        {
            ServiceResponse<int> response = new();

            try
            {
                Train train = await _context.Trains.Find(x => x.Id == new ObjectId(id)).FirstOrDefaultAsync();

                if (train == null)
                {
                    response.Success = false;
                    response.Message = "Train not found.";
                    return response;
                }

                train.Name = data.Name;
                train.DepartureStation = data.DepartureStation;
                train.ArrivalStation = data.ArrivalStation;

                await _context.Trains.ReplaceOneAsync(x => x.Id == new ObjectId(id), train);

                response.Success = true;
                response.Message = "Train updated.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<int>> UpdateTrainActiveStatus(string trainId, bool status)
        {
            ServiceResponse<int> response = new();

            Train train = await _context.Trains.Find(x => x.Id == new ObjectId(trainId)).FirstOrDefaultAsync();

            if (train == null)
            {
                response.Success = false;
                response.Message = "Train not found.";
                return response;
            }

            if (status == false)
            {
                if (await HasReservations(trainId))
                {
                    response.Success = false;
                    response.Message = "Train has reservations. Cannot deactivate.";
                    return response;
                }
                else
                {
                    train.IsPublished = false;
                }
            }

            train.IsActive = status;

            await _context.Trains.ReplaceOneAsync(x => x.Id == new ObjectId(trainId), train);

            response.Success = true;
            response.Message = "Train active status updated.";

            return response;
        }

        public async Task<ServiceResponse<int>> UpdateTrainPublishedStatus(string trainId, bool status)
        {
            ServiceResponse<int> response = new();

            Train train = await _context.Trains.Find(x => x.Id == new ObjectId(trainId)).FirstOrDefaultAsync();

            if (train == null)
            {
                response.Success = false;
                response.Message = "Train not found.";
                return response;
            }

            if (status == false)
            {
                if (await HasReservations(trainId))
                {
                    response.Success = false;
                    response.Message = "Train has reservations. Cannot unpublish.";
                    return response;
                }
            }
            else
            {
                if (train.IsActive == false)
                {
                    response.Success = false;
                    response.Message = "Train is not active. Cannot publish.";
                    return response;
                }
            }

            train.IsPublished = status;

            await _context.Trains.ReplaceOneAsync(x => x.Id == new ObjectId(trainId), train);

            response.Success = true;
            response.Message = "Train published status updated.";

            return response;
        }

        public async Task<ServiceResponse<int>> DeleteTrain(string id)
        {
            ServiceResponse<int> response = new();

            try
            {
                DeleteResult result = await _context.Trains.DeleteOneAsync(x => x.Id == new ObjectId(id));

                if (result.DeletedCount == 0)
                {
                    response.Success = false;
                    response.Message = "Train not found.";
                    return response;
                }

                response.Success = true;
                response.Message = "Train deleted.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<int>> AddTrainSchedule(CreateTrainSchedule trainSchedule)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<int>> UpdateTrainSchedule(string id, UpdateTrainSchedule trainSchedule)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<int>> DeleteTrainSchedule(string id)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> HasReservations(string trainId)
        {
            if (await _context.Reservations.Find(x => x.TrainId == new ObjectId(trainId)).FirstOrDefaultAsync() != null)
            {
                return true;
            }

            return false;
        }
    }
}
