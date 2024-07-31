using IPSMDB.Models;

namespace IPSMDB.Interfaces
{
    public interface IAppUserRepository
    {
        ICollection<AppUser> GetAppUsers();
        AppUser GetAppUser(int id);
    }
}