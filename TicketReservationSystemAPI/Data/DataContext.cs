using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TicketReservationSystemAPI.Models;

namespace TicketReservationSystemAPI.Data
{
    public class DataContext
    {
        private readonly IMongoDatabase _database;

        public DataContext(IOptions<DbSettings> options)
        {
            var settings = options.Value;
            var client = new MongoClient(settings.ConnectionURI);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<Traveler> Travelers => _database.GetCollection<Traveler>("Travelers");
        public IMongoCollection<Agent> Agents => _database.GetCollection<Agent>("Agents");
        public IMongoCollection<Admin> Admins => _database.GetCollection<Admin>("Admins");
        public IMongoCollection<Train> Trains => _database.GetCollection<Train>("Trains");
        public IMongoCollection<TrainSchedule> TrainSchedules => _database.GetCollection<TrainSchedule>("TrainSchedules");
        public IMongoCollection<Reservation> Reservations => _database.GetCollection<Reservation>("Reservations");
    }
}
