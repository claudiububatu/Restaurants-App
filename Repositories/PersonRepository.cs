using IPSMDB.Context;
using IPSMDB.Controllers.Models;
using IPSMDB.Interfaces;
using IPSMDB.Models;

namespace IPSMDB.Repositories
{
    public class FoodPlaceEqualityComparer : IEqualityComparer<FoodPlace>
    {
        public bool Equals(FoodPlace x, FoodPlace y)
        {
            if (x == null || y == null)
                return false;

            return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(FoodPlace obj)
        {
            return obj.Name.ToLower().GetHashCode();
        }
    }
    public class PersonRepository : IPersonRepository
    {
        private readonly DatabaseContext _databaseContext;
        public PersonRepository(DatabaseContext context)
        {
            _databaseContext = context;
        }

        public bool CreatePerson(Person person)
        {
            _databaseContext.Add(person);
            return Save();
        }

        public bool DeletePerson(Person person)
        {
            _databaseContext.Remove(person);
            return Save();
        }

        public ICollection<PersonFriendship> GetAllFriendships()
        {
            return _databaseContext.PersonFriendship.ToList();
        }

        public ICollection<FoodPlace> GetFoodPlacesById(int id)
        {
            return _databaseContext
                .Person
                .Where(p => p.Id == id)
                .SelectMany(p => p.FoodPlaces)
                .ToList();
        }

        public ICollection<Person> GetPerson()
        {
            return _databaseContext.Person.OrderBy(p => p.Id).ToList();
        }

        public Person GetPerson(int id)
        {
            return _databaseContext.Person.Where(p => p.Id == id).FirstOrDefault();
        }

        public Person GetPerson(string lastName, string firstName)
        {
            return _databaseContext.Person
                .Where(p => p.LastName.ToLower().Trim() == lastName.ToLower().Trim()
                && p.FirstName.ToLower().Trim() == firstName.ToLower().Trim())
                .FirstOrDefault();
        }

        public ICollection<Person> GetPersonsByNameMatch(string name)
        {
            return _databaseContext.Person
                .Where(p => p.LastName.ToLower().Contains(name.ToLower().Trim())
                || p.FirstName.ToLower().Contains(name.ToLower().Trim()))
                .ToList();
        }

        public bool CreateFriendship(int personId, int friendId)
        {
            var person = _databaseContext.Person.FirstOrDefault(p => p.Id == personId);
            var friend = _databaseContext.Person.FirstOrDefault(p => p.Id == friendId);

            if (person == null || friend == null)
            {
                return false;
            }

            var existingFriendship1 = _databaseContext.PersonFriendship
                .Any(pf => pf.PersonId == personId && pf.FriendId == friendId);

            var existingFriendship2 = _databaseContext.PersonFriendship
                .Any(pf => pf.PersonId == friendId && pf.FriendId == personId);

            if (existingFriendship1 || existingFriendship2)
            {
                return false;
            }

            var friendship1 = new PersonFriendship()
            {
                Person = person,
                Friend = friend,
            };

            var friendship2 = new PersonFriendship()
            {
                Person = friend,
                Friend = person,
            };

            _databaseContext.Add(friendship1);
            _databaseContext.Add(friendship2);
            return Save();
        }

        public ICollection<Person> GetFriends(string firstName, string lastName)
        {
            var person = _databaseContext.Person.FirstOrDefault(p => p.FirstName == firstName && p.LastName == lastName);

            if (person == null)
            {
                return null;
            }

            var friendships = _databaseContext.PersonFriendship.Where(pf => pf.PersonId == person.Id).ToList();

            var friends = new List<Person>();

            foreach (var friendship in friendships)
            {
                var id = friendship.FriendId;
                var friend = GetPerson(id);
                friends.Add(friend);
            }

            return friends;
        }

        public Person GetPersonByLastName(string lastName)
        {
            return _databaseContext.Person.Where(p => p.LastName == lastName).FirstOrDefault();
        }

        public Person GetPersonByFirstName(string firstName)
        {
            return _databaseContext.Person.Where(p => p.FirstName == firstName).FirstOrDefault();
        }

        public decimal GetPersonRating(int personId)
        {
            var review = _databaseContext.ServiceReview.Where(p => p.Person.Id == personId);

            if (review.Count() <= 0)
            {
                return 0;
            }

            return (review.Sum(r => r.CustomerKindness) / (decimal)review.Count());
        }

        public decimal GetPersonRating(string name)
        {
            var review = _databaseContext.ServiceReview.Where(p => p.Person.FirstName == name || p.Person.LastName == name);

            if (review.Count() <= 0)
            {
                return 0;
            }

            return (review.Sum(r => r.CustomerKindness) / (decimal)review.Count());
        }

        public bool PersonExists(int personId)
        {
            return _databaseContext.Person.Any(p => p.Id == personId);
        }

        public bool PersonExists(string lastName, string firstName)
        {
            return _databaseContext.Person
                .Any(p => p.LastName.ToLower().Trim() == lastName.ToLower().Trim()
                && p.FirstName.ToLower().Trim() == firstName.ToLower().Trim());
        }

        public bool PersonExists(string name)
        {
            return _databaseContext.Person
                .Any(p => p.LastName.ToLower().Trim() == name.ToLower().Trim()
                || p.FirstName.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool Save()
        {
            var saved = _databaseContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdatePerson(Person person)
        {
            _databaseContext.Update(person);
            return Save();
        }

        public IEnumerable<FoodPlace>? GetFriendsNewFoodPlaces(string firstName, string lastName)
        {
            if (!PersonExists(lastName, firstName))
            {
                return null;
            }

            var friends = GetFriends(firstName, lastName);

            var currentPerson = GetPerson(lastName, firstName);

            var myFoodPlaces = GetFoodPlacesById(currentPerson.Id);

            var myFoodPlaceNames = new HashSet<string>(myFoodPlaces.Select(fp => fp.Name), StringComparer.OrdinalIgnoreCase);

            var missingList = new HashSet<FoodPlace>(new FoodPlaceEqualityComparer());

            foreach (var friend in friends)
            {
                if (!PersonExists(friend.LastName, friend.FirstName))
                    return null;

                var person = GetPerson(friend.LastName, friend.FirstName);

                var friendFoodPlaces = GetFoodPlacesById(person.Id);

                foreach (var friendFoodPlace in friendFoodPlaces)
                {
                    if (!myFoodPlaceNames.Contains(friendFoodPlace.Name))
                    {
                        missingList.Add(friendFoodPlace);
                    }
                }
            }

            return missingList;
        }

        public IEnumerable<int>? GetFavoriteFoodCategoryIds(int personId)
        {
            if (!PersonExists(personId))
                return null;

            var foodPlaces = GetFoodPlacesById(personId);

            if (foodPlaces == null || !foodPlaces.Any())
                return null;

            var favoriteCategoryIds = foodPlaces
                .GroupBy(fp => fp.FoodPlaceCategoryId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key);

            return favoriteCategoryIds;
        }
    }
}