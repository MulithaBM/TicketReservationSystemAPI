using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.enums;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public AdminService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<string>> Login(AdminLogin data)
        {
            ServiceResponse<string> response = new();

            Admin admin = await _context.Admins.Find(x => x.Email.ToLower() == data.Email.ToLower()).FirstOrDefaultAsync();

            if (admin == null)
            {
                response.Success = false;
                response.Message = "Invalid email or password";
                return response;
            }
            else if (!VerifyPasswordHash(data.Password, admin.PasswordHash, admin.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Invalid email or password";
                return response;
            }
            else
            {
                response.Data = CreateToken(admin);
                response.Success = true;
                response.Message = "Login successful";
            }

            return response;
        }

        public async Task<ServiceResponse<string>> Register(AdminRegistration data)
        {
            ServiceResponse<string> response = new();

            if (await UserExistsEmail(data.Email))
            {
                response.Success = false;
                response.Message = "Account with the Email already exists";
                return response;
            }

            CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

            Admin admin = new()
            {
                Name = data.Name,
                Email = data.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                ContactNo = data.ContactNo,
            };

            await _context.Admins.InsertOneAsync(admin);

            response.Success = true;
            response.Message = "Account created successfully";

            return response;
        }

        public async Task<ServiceResponse<AdminReturn>> GetAccount(string userId)
        {
            ServiceResponse<AdminReturn> response = new();

            Admin admin = await _context.Admins.Find(x => x.Id.ToString() == userId).FirstOrDefaultAsync();

            if (admin == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            AdminReturn adminReturn = _mapper.Map<AdminReturn>(admin);

            response.Data = adminReturn;
            response.Success = true;

            return response;
        }

        public async Task<ServiceResponse<AdminReturn>> UpdateAccount(string userId, AdminUpdate data)
        {
            ServiceResponse<AdminReturn> response = new();

            Admin admin = await _context.Admins.Find(x => x.Id.ToString() == userId).FirstOrDefaultAsync();

            if (admin == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            if (data.Name != null)
            {
                admin.Name = data.Name;
            }

            if (data.ContactNo != null)
            {
                admin.ContactNo = data.ContactNo;
            }

            if (data.PreviousPassword != null && data.Password != null)
            {
                if (!VerifyPasswordHash(data.PreviousPassword, admin.PasswordHash, admin.PasswordSalt))
                {
                    response.Success = false;
                    response.Message = "Current password is wrong";
                    return response;
                }

                CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);
                admin.PasswordHash = passwordHash;
                admin.PasswordSalt = passwordSalt;
            }

            await _context.Admins.ReplaceOneAsync(x => x.Id == admin.Id, admin);

            AdminReturn adminReturn = _mapper.Map<AdminReturn>(admin);

            response.Data = adminReturn;
            response.Success = true;
            response.Message = "Account updated successfully";

            return response;
        }

        public async Task<ServiceResponse<string>> DeleteAccount(string userId)
        {
            ServiceResponse<string> response = new();

            Admin admin = await _context.Admins.Find(x => x.Id.ToString() == userId).FirstOrDefaultAsync();

            if (admin == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            await _context.Admins.DeleteOneAsync(x => x.Id == admin.Id);

            response.Success = true;
            response.Message = "Account deleted successfully";

            return response;
        }

        private async Task<bool> UserExistsEmail(string email)
        {
            if (await _context.Admins.Find(x => x.Email.ToLower() == email.ToLower()).AnyAsync())
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

        private string CreateToken(Admin admin)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Email),
                new Claim(ClaimTypes.Role, UserRole.Admin.ToString())
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
