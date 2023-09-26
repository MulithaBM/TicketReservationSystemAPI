using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Data
{
    public class AuthRepository<T> : IAuthRepository<T>
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public Task<ServiceResponse<string>> Login(T user)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<int>> Register(T user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExists(string email)
        {
            throw new NotImplementedException();
        }

        //public async Task<bool> UserExists(string username)
        //{
        //    if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        //{
        //    using (var hmac = new System.Security.Cryptography.HMACSHA512())
        //    {
        //        passwordSalt = hmac.Key;
        //        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //    }
        //}

        //private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        //{
        //    using (var hmac = new HMACSHA512(passwordSalt))
        //    {
        //        var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //        return computeHash.SequenceEqual(passwordHash);
        //    }
        //}

        //private string CreateToken(User user)
        //{
        //    List<Claim> claims = new List<Claim> {
        //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //        new Claim(ClaimTypes.Name, user.Username),
        //        new Claim(ClaimTypes.Role, user.Role)
        //    };
        //    SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

        //    SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        //    SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(claims),
        //        Expires = DateTime.Now.AddDays(1),
        //        SigningCredentials = creds
        //    };

        //    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        //    SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}
    }
}
