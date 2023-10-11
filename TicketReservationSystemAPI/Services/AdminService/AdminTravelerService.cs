// File name: AdminTravelerService.cs
// <summary>
// Description: A brief description of the file's purpose.
// </summary>
// <author>MulithaBM</author>
// <created>11/10/2023</created>
// <modified>11/10/2023</modified>

using System.Security.Cryptography;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;
using MongoDB.Driver;
using AutoMapper;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public class AdminTravelerService : IAdminTravelerService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public AdminTravelerService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<string>> CreateAccount(AdminTravelerRegistration data)
        {
            ServiceResponse<string> response = new();

            if (await UserExistsNIC(data.NIC))
                return CreateErrorResponse(response, "Account with this NIC already exists");

            if (await UserExistsEmail(data.Email))
                return CreateErrorResponse(response, "Account with this email already exists");

            CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

            Traveler traveler = new()
            {
                NIC = data.NIC,
                Name = data.Name,
                Email = data.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsActive = true,
            };

            await _context.Travelers.InsertOneAsync(traveler);

            response.Success = true;
            response.Message = "Registration successful";

            return response;
        }

        public async Task<ServiceResponse<List<AdminGetTraveler>>> GetAccounts(bool? status)
        {
            ServiceResponse<List<AdminGetTraveler>> response = new();

            List<Traveler> travelers;

            if (status == null)
            {
                travelers = await _context.Travelers.Find(x => true).ToListAsync();
            }
            else
            {
                travelers = await _context.Travelers.Find(x => x.IsActive == status).ToListAsync();
            }

            if (travelers.Count == 0)
            {
                response.Success = true;
                response.Message = "No travelers found";

                return response;
            }

            response.Data = _mapper.Map<List<AdminGetTraveler>>(travelers);
            response.Success = true;

            return response;
        }

        public async Task<ServiceResponse<AdminGetTravelerWithReservations>> GetAccount(string nic)
        {
            ServiceResponse<AdminGetTravelerWithReservations> response = new();

            Traveler traveler = await _context.Travelers.Find(x => x.NIC.ToLower() == nic.ToLower()).FirstOrDefaultAsync();

            if (traveler == null)
                return CreateErrorResponse(response, "Account not found");

            AdminGetTravelerWithReservations travelerWithReservations = _mapper.Map<AdminGetTravelerWithReservations>(traveler);

            DateTime current = DateTime.Now;
            DateOnly currentDate = DateOnly.FromDateTime(current);
            TimeOnly currentTime = TimeOnly.FromDateTime(current);

            var filterBuilder = Builders<Reservation>.Filter;
            var filter = filterBuilder.Empty;

            filter &= filterBuilder.Eq(reservation => reservation.TravelerId, nic);
            filter &= filterBuilder.Eq(reservation => reservation.IsCancelled, false);
            filter &= filterBuilder.Gte(reservation => reservation.ReservationDate, currentDate);
            filter &= filterBuilder.Gte(reservation => reservation.DepartureTime, currentTime);

            List<Reservation> reservations = await _context.Reservations
                .Find(filter)
                .SortBy(x => x.ReservationDate)
                .ThenBy(x => x.DepartureTime)
                .ToListAsync();

            List<AdminGetReservation> adminGetReservations = _mapper.Map<List<AdminGetReservation>>(reservations);

            response.Data = travelerWithReservations;
            response.Success = true;

            return response;
        }

        public async Task<ServiceResponse<AdminGetTraveler>> UpdateAccount(string nic, AdminTravelerUpdate data)
        {
            ServiceResponse<AdminGetTraveler> response = new();

            Traveler traveler = await _context.Travelers.Find(x => x.NIC.ToLower() == nic.ToLower()).FirstOrDefaultAsync();

            if (traveler == null)
            {
                response.Success = false;
                response.Message = "Account not found";
                return response;
            }

            if (data.Email != null)
            {
                traveler.Email = data.Email;
            }

            if (data.ContactNo != null)
            {
                traveler.ContactNo = data.ContactNo;
            }

            if (data.Password != null)
            {
                CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

                traveler.PasswordHash = passwordHash;
                traveler.PasswordSalt = passwordSalt;
            }

            await _context.Travelers.ReplaceOneAsync(x => x.NIC.ToLower() == nic.ToLower(), traveler);

            response.Data = _mapper.Map<AdminGetTraveler>(traveler);
            response.Success = true;
            response.Message = "Account updated successfully";

            return response;
        }

        public async Task<ServiceResponse<AdminGetTraveler>> UpdateActiveStatus(string nic)
        {
            ServiceResponse<AdminGetTraveler> response = new();

            Traveler traveler = await _context.Travelers.Find(x => x.NIC.ToLower() == nic.ToLower()).FirstOrDefaultAsync();

            if (traveler == null)
            {
                response.Success = false;
                response.Message = "Account not found";
                return response;
            }

            traveler.IsActive = !traveler.IsActive;

            await _context.Travelers.ReplaceOneAsync(x => x.NIC.ToLower() == nic.ToLower(), traveler);

            response.Data = _mapper.Map<AdminGetTraveler>(traveler);
            response.Success = true;
            response.Message = "Account " + (traveler.IsActive ? "activated" : "deactivated") + " successfully";

            return response;
        }

        public async Task<ServiceResponse<string>> DeleteAccount(string userId)
        {
            ServiceResponse<string> response = new();

            Traveler traveler = await _context.Travelers.Find(x => x.NIC.ToLower() == userId.ToLower()).FirstOrDefaultAsync();

            if (traveler == null)
            {
                response.Success = false;
                response.Message = "Account not found";
                return response;
            }

            await _context.Travelers.DeleteOneAsync(x => x.NIC.ToLower() == userId.ToLower());

            response.Success = true;
            response.Message = "Account deleted successfully";

            return response;
        }
        
        public async Task<bool> UserExistsNIC(string nic)
        {
            if (await _context.Travelers.Find(x => x.NIC.ToLower() == nic.ToLower()).AnyAsync())
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UserExistsEmail(string email)
        {
            if (await _context.Travelers.Find(x => x.Email.ToLower() == email.ToLower()).AnyAsync())
            {
                return true;
            }

            return false;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static ServiceResponse<T> CreateErrorResponse<T>(ServiceResponse<T> response, string message)
        {
            response.Success = false;
            response.Message = message;
            return response;
        }
    }
}
