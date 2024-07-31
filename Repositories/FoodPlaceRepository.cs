using IPSMDB.Context;
using IPSMDB.Interfaces;
using IPSMDB.Models;

namespace IPSMDB.Repositories
{
    public class FoodPlaceRepository : IFoodPlaceRepository
    {
        private readonly DatabaseContext _databaseContext;
        public FoodPlaceRepository(DatabaseContext context)
        {
            _databaseContext = context;
        }
        public bool CreateFoodPlace(FoodPlace foodPlace)
        {
            _databaseContext.Add(foodPlace);
            return Save();
        }

        public bool DeleteFoodPlace(FoodPlace foodPlace)
        {
            _databaseContext.Remove(foodPlace);
            return Save();
        }

        public bool FoodPlaceExists(int id)
        {
            return _databaseContext.FoodPlace.Any(f => f.Id == id);
        }

        public bool FoodPlaceExists(string name)
        {
            return _databaseContext.FoodPlace.Any(f => f.Name == name);
        }

        public FoodPlace GetFoodPlace(int id)
        {
            return _databaseContext.FoodPlace.Where(fp => fp.Id == id).FirstOrDefault();
        }

        public FoodPlace GetFoodPlace(string name)
        {
            return _databaseContext.FoodPlace.Where(fp => fp.Name == name).FirstOrDefault();
        }

        public ICollection<FoodPlace> GetFoodPlaces()
        {
            return _databaseContext.FoodPlace
                .OrderBy(fp => fp.Id)
                .ToList();
        }

        public ICollection<FoodPlace> GetFoodPlacesByPersonId(int personId)
        {
            return _databaseContext.FoodPlace.Where(fp => fp.PersonId == personId).ToList();
        }

        public ICollection<Location> GetLocationsByFoodPlaceName(string name)
        {
            return _databaseContext
                .FoodPlace
                .Where(fp => fp.Name == name)
                .SelectMany(fp => fp.Locations)
                .ToList();
        }

        public ICollection<CustomerReview> GetPersonReviewsForFoodPlaceName(string name)
        {
            return _databaseContext
                .FoodPlace
                .Where(fp => fp.Name == name)
                .SelectMany(fp => fp.ReviewsReceived)
                .ToList();
        }

        public bool Save()
        {
            var saved = _databaseContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateFoodPlace(FoodPlace foodPlace)
        {
            _databaseContext.Update(foodPlace);
            return Save();
        }
    }
}