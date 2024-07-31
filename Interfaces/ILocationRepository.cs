using IPSMDB.Models;

namespace IPSMDB.Interfaces
{
    public interface ILocationRepository
    {
        ICollection<Location> GetLocations();
        Location GetLocation(int id);
        Location GetLocation(string foodPlaceName);
        bool LocationExists(int locationId);
        bool CreateLocation(Location location);
        bool UpdateLocation(Location location);
        bool DeleteLocation(Location location);
        bool Save();
    }
}