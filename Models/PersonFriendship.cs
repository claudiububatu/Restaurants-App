using IPSMDB.Models;

namespace IPSMDB.Models
{
    public class PersonFriendship
    {
        public int PersonId { get; set; }
        public int FriendId { get; set; }
        public Person Person { get; set; }
        public Person Friend { get; set; }
    }
}