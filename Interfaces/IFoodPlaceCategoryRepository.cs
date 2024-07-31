using IPSMDB.Models;

namespace IPSMDB.Interfaces
{
    public interface IFoodPlaceCategoryRepository
    {
        ICollection<FoodPlaceCategory> GetFoodPlaceCategories();
        FoodPlaceCategory GetFoodPlaceCategory(int id);
        FoodPlaceCategory GetFoodPlaceCategory(string name);
        ICollection<FoodPlace> GetFoodPlacesByCategory(string name);
        bool FoodPlaceCategoryExists(int id);
        bool FoodPlaceCategoryExists(string name);
        bool CreateFoodPlaceCategory(FoodPlaceCategory category);
        bool UpdateFoodPlaceCategory(FoodPlaceCategory foodPlaceCategory);
        bool DeleteFoodPlaceCategory(FoodPlaceCategory foodPlaceCategory);
        bool Save();
    }
}