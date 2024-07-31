using IPSMDB.Context;
using IPSMDB.Interfaces;
using IPSMDB.Models;

namespace IPSMDB.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly DatabaseContext _databaseContext;
        public LocationRepository(DatabaseContext context)
        {
            _databaseContext = context;
        }

        public bool CreateLocation(Location location)
        {
            _databaseContext.Add(location);
            return Save();
        }

        public bool DeleteLocation(Location location)
        {
            _databaseContext.Remove(location);
            return Save();
        }

        public Location GetLocation(int id)
        {
            return _databaseContext.Location.Where(l => l.Id == id).FirstOrDefault();
        }

        public Location GetLocation(string foodPlaceName)
        {
            return _databaseContext.Location.Where(l => l.FoodPlace.Name == foodPlaceName).FirstOrDefault();
        }

        public ICollection<Location> GetLocations()
        {
            return _databaseContext.Location.OrderBy(p => p.Id).ToList();
        }

        public bool LocationExists(int locationId)
        {
            return _databaseContext.Location.Any(l => l.Id == locationId);
        }

        public bool Save()
        {
            var saved = _databaseContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateLocation(Location location)
        {
            _databaseContext.Update(location);
            return Save();
        }
    }
}