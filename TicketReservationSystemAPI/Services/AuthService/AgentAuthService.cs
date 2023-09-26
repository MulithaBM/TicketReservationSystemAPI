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
    public class AgentAuthService : IAgentAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AgentAuthService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<string>> Login(AgentLogin data)
        {
            ServiceResponse<string> response = new();

            Agent agent = await _context.Agents.Find(x => x.Email.ToLower() == data.Email.ToLower()).FirstOrDefaultAsync();

            if (agent == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            else if (!VerifyPasswordHash(data.Password, agent.PasswordHash, agent.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password";
                return response;
            }
            else
            {
                response.Data = CreateToken(agent);
                response.Success = true;
                response.Message = "Login successful";
            }

            return response;
        }

        public async Task<ServiceResponse<int>> Register(AgentRegistration data)
        {
            ServiceResponse<int> response = new();

            if (await UserExists(data.Email))
            {
                response.Success = false;
                response.Message = "User already exists";
                return response;
            }

            CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

            Agent newAgent = new()
            {
                Name = data.Name,
                Email = data.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                ContactNo = data.ContactNo,
            };

            await _context.Agents.InsertOneAsync(newAgent);

            response.Success = true;
            response.Message = "User created successfully";

            return response;
        }

        public async Task<bool> UserExists(string email)
        {
            if (await _context.Agents.Find(x => x.Email.ToLower() == email.ToLower()).AnyAsync())
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

        private string CreateToken(Agent agent)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, agent.Id.ToString()),
                new Claim(ClaimTypes.Email, agent.Email),
                new Claim(ClaimTypes.Role, UserRole.TravelAgent.ToString())
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
