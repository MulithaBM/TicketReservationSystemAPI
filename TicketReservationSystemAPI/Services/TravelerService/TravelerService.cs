using MongoDB.Driver;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
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

        public async Task<ServiceResponse<Traveler>> GetProfile(string userId)
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
                response.Message = "User found";
            }

            return response;
        }
    }
}
