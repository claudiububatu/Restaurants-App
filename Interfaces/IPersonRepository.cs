using System;
using IPSMDB.Models;

namespace IPSMDB.Interfaces
{
    public interface IPersonRepository
    {
        ICollection<Person> GetPerson();
        Person GetPerson(int id);
        Person GetPerson(string lastName, string firstName);
        ICollection<Person> GetPersonsByNameMatch(string name);
        Person GetPersonByLastName(string lastName);
        Person GetPersonByFirstName(string firstName);
        decimal GetPersonRating(int personId);
        decimal GetPersonRating(string name);
        ICollection<PersonFriendship> GetAllFriendships();
        ICollection<Person> GetFriends(string firstName, string lastName);
        ICollection<FoodPlace> GetFoodPlacesById(int id);
        IEnumerable<FoodPlace>? GetFriendsNewFoodPlaces(string firstName, string lastName);
        IEnumerable<int>? GetFavoriteFoodCategoryIds(int personId);
        bool PersonExists(int personId);
        bool PersonExists(string lastName, string firstName);
        bool PersonExists(string name);
        bool CreatePerson(Person person);
        bool CreateFriendship(int personId, int friendId);
        bool UpdatePerson(Person person);
        bool DeletePerson(Person person);
        bool Save();
    }
}