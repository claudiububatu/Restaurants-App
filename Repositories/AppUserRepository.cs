using IPSMDB.Context;
using IPSMDB.Interfaces;
using IPSMDB.Models;
// using System.Data.Entity;

namespace IPSMDB.Repositories
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly DatabaseContext _databaseContext;
        public AppUserRepository(DatabaseContext context)
        {
            _databaseContext = context;
        }
        public ICollection<AppUser> GetAppUsers()
        {
            return _databaseContext.AppUsers.OrderBy(au => au.Id).ToList();
        }

        public AppUser GetAppUser(int id)
        {
            return _databaseContext.AppUsers.Where(au => au.PersonId == id).FirstOrDefault();
        }
    }
}