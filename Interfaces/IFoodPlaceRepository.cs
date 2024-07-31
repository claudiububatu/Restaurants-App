using IPSMDB.Models;

namespace IPSMDB.Interfaces
{
    public interface IFoodPlaceRepository
    {
        ICollection<FoodPlace> GetFoodPlaces();
        ICollection<FoodPlace> GetFoodPlacesByPersonId(int personId);
        FoodPlace GetFoodPlace(int id);
        FoodPlace GetFoodPlace(string name);
        ICollection<Location> GetLocationsByFoodPlaceName(string name);
        ICollection<CustomerReview> GetPersonReviewsForFoodPlaceName(string name);
        bool FoodPlaceExists(int id);
        bool FoodPlaceExists(string name);
        bool CreateFoodPlace(FoodPlace foodPlace);
        bool UpdateFoodPlace(FoodPlace foodPlace);
        bool DeleteFoodPlace(FoodPlace foodPlace);
        bool Save();
    }
}