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
            {
                response.Success = false;
                response.Message = "User already exists";
                return response;
            }

            if (await UserExistsEmail(data.Email))
            {
                response.Success = false;
                response.Message = "Account with this email already exists";
                return response;
            }

            CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

            Traveler traveler = new()
            {
                Name = data.Name,
                NIC = data.NIC,
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

        public async Task<ServiceResponse<List<AdminTravelerReturn>>> GetAccounts(bool? status = null)
        {
            ServiceResponse<List<AdminTravelerReturn>> response = new();

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

            response.Data = _mapper.Map<List<AdminTravelerReturn>>(travelers);
            response.Success = true;

            return response;
        }

        public async Task<ServiceResponse<AdminTravelerReturn>> GetAccount(string nic)
        {
            ServiceResponse<AdminTravelerReturn> response = new();

            Traveler traveler = await _context.Travelers.Find(x => x.NIC.ToLower() == nic.ToLower()).FirstOrDefaultAsync();

            if (traveler == null)
            {
                response.Success = false;
                response.Message = "Account not found";
                return response;
            }

            response.Data = _mapper.Map<AdminTravelerReturn>(traveler);
            response.Success = true;

            return response;
        }

        public async Task<ServiceResponse<AdminTravelerReturn>> UpdateAccount(string nic, AdminTravelerUpdate data)
        {
            ServiceResponse<AdminTravelerReturn> response = new();

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

            response.Data = _mapper.Map<AdminTravelerReturn>(traveler);
            response.Success = true;
            response.Message = "Account updated succesfully";

            return response;
        }

        public async Task<ServiceResponse<AdminTravelerReturn>> UpdateActiveStatus(string nic)
        {
            ServiceResponse<AdminTravelerReturn> response = new();

            Traveler traveler = await _context.Travelers.Find(x => x.NIC.ToLower() == nic.ToLower()).FirstOrDefaultAsync();

            if (traveler == null)
            {
                response.Success = false;
                response.Message = "Account not found";
                return response;
            }

            traveler.IsActive = !traveler.IsActive;

            await _context.Travelers.ReplaceOneAsync(x => x.NIC.ToLower() == nic.ToLower(), traveler);

            response.Data = _mapper.Map<AdminTravelerReturn>(traveler);
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
    }
}
