using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.enums;
using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.AuthService
{
    public class TravelerAuthservice : ITravelerAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public TravelerAuthservice(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

            if (await UserExists(data.Email))
            {
                response.Success = false;
                response.Message = "User already exists";
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
            };

            await _context.Travelers.InsertOneAsync(newTraveler);

            response.Success = true;
            response.Message = "User created successfully";

            return response;
        }

        public async Task<bool> UserExists(string email)
        {
            if (await _context.Travelers.Find(x => x.Email.ToLower() == email.ToLower()).AnyAsync())
            {
                return true;
            }

            return false;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
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
                new Claim(ClaimTypes.NameIdentifier, traveler.NIC.ToString()),
                new Claim(ClaimTypes.Name, traveler.Email),
                new Claim(ClaimTypes.Role, UserRole.Traveler.ToString())
            };
            SymmetricSecurityKey key = new(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

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
