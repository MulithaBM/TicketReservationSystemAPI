using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.enums;
using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.TravelerService
{
    public class TravelerService : ITravelerService
    {
        private readonly DataContext _context;

        public TravelerService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<string>> Login(TravelerLogin data)
        {
            ServiceResponse<string> response = new();

            Traveler traveler = await _context.Travelers.Find(x => x.Email.ToLower() == data.Email.ToLower()).FirstOrDefaultAsync();

            if (traveler == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            else if (!VerifyPasswordHash(data.Password, traveler.PasswordHash, traveler.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password";
                return response;
            }
            else if (!traveler.IsActive)
            {
                response.Success = false;
                response.Message = "Account deactivated";
                return response;
            }
            else
            {
                response.Data = CreateToken(traveler);
                response.Success = true;
                response.Message = "Login successful";
            }

            return response;
        }

        public async Task<ServiceResponse<int>> Register(TravelerRegistration data)
        {
            ServiceResponse<int> response = new();

            if (await UserExistsNIC(data.NIC))
            {
                response.Success = false;
                response.Message = "Account with the NIC already exists";
                return response;
            }
            if (await UserExistsEmail(data.Email))
            {
                response.Success = false;
                response.Message = "Account with the Email already exists";
                return response;
            }

            CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

            Traveler newTraveler = new()
            {
                NIC = data.NIC,
                Name = data.Name,
                Email = data.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                ContactNo = data.ContactNo,
                IsActive = true
            };

            await _context.Travelers.InsertOneAsync(newTraveler);

            response.Success = true;
            response.Message = "User created successfully";

            return response;
        }

        public async Task<ServiceResponse<Traveler>> GetAccount(string userId)
        {
            ServiceResponse<Traveler> response = new();

            Traveler traveler = await _context.Travelers.Find(x => x.NIC == userId).FirstOrDefaultAsync();

            if (traveler == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            else
            {
                response.Data = traveler;
                response.Success = true;
            }

            return response;
        }

        public async Task<ServiceResponse<int>> UpdateAccount(string userId, TravelerUpdate data)
        {
            ServiceResponse<int> response = new();

            Traveler traveler = await _context.Travelers.Find(x => x.NIC == userId).FirstOrDefaultAsync();

            if (traveler == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            else
            {
                // Check if null check is necessary
                if (data.Name != null) traveler.Name = data.Name;
                if (data.ContactNo != null) traveler.ContactNo = data.ContactNo;

                await _context.Travelers.ReplaceOneAsync(x => x.NIC == userId, traveler);

                response.Success = true;
                response.Message = "User updated successfully";
            }

            return response;
        }

        public async Task<ServiceResponse<int>> DeactivateAccount(string userId)
        {
            ServiceResponse<int> response = new();

            Traveler traveler = await _context.Travelers.Find(x => x.NIC == userId).FirstOrDefaultAsync();

            if (traveler == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            else
            {
                traveler.IsActive = false;

                await _context.Travelers.ReplaceOneAsync(x => x.NIC == userId, traveler);

                response.Success = true;
                response.Message = "User account deactivated succesfully";
            }

            return response;
        }

        //private
        public async Task<bool> UserExistsNIC(string nic)
        {
            if (await _context.Travelers.Find(x => x.NIC.ToLower() == nic.ToLower()).AnyAsync())
            {
                return true;
            }

            return false;
        }

        //private
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

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computeHash.SequenceEqual(passwordHash);
        }

        private string CreateToken(Traveler traveler)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, traveler.NIC),
                new Claim(ClaimTypes.Name, traveler.Email),
                new Claim(ClaimTypes.Role, SystemRole.Traveler.ToString())
            };
            SymmetricSecurityKey key = new(System.Text.Encoding.UTF8.GetBytes(JWTSettings.Token));

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
