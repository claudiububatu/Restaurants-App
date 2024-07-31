using IPSMDB.Context;
using IPSMDB.Interfaces;
using IPSMDB.Models;
using IPSMDB.Context;
using IPSMDB.Interfaces;
using IPSMDB.Models;

namespace IPSMDB.Repositories
{
    public class FoodPlaceCategoryRepository : IFoodPlaceCategoryRepository
    {
        private readonly DatabaseContext _databaseContext;

        public FoodPlaceCategoryRepository(DatabaseContext context)
        {
            _databaseContext = context;
        }

        public ICollection<FoodPlace> GetFoodPlacesByCategory(string name)
        {
            return _databaseContext
                .FoodPlaceCategory
                .Where(fpc => fpc.Name == name)
                .SelectMany(fp => fp.FoodPlaces)
                .ToList();
        }

        public ICollection<FoodPlaceCategory> GetFoodPlaceCategories()
        {
            return _databaseContext.FoodPlaceCategory.OrderBy(fpc => fpc.Id).ToList();
        }

        public FoodPlaceCategory GetFoodPlaceCategory(int id)
        {
            return _databaseContext.FoodPlaceCategory.Where(fpc => fpc.Id == id).FirstOrDefault();
        }

        public FoodPlaceCategory GetFoodPlaceCategory(string name)
        {
            return _databaseContext.FoodPlaceCategory.Where(fpc => fpc.Name == name).FirstOrDefault();
        }

        public bool FoodPlaceCategoryExists(int id)
        {
            return _databaseContext.FoodPlaceCategory.Any(fpc => fpc.Id == id);
        }

        public bool FoodPlaceCategoryExists(string name)
        {
            return _databaseContext.FoodPlaceCategory.Any(fpc => fpc.Name == name);
        }

        public bool CreateFoodPlaceCategory(FoodPlaceCategory category)
        {
            _databaseContext.Add(category);
            return Save();
        }

        public bool Save()
        {
            var saved = _databaseContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateFoodPlaceCategory(FoodPlaceCategory foodPlaceCategory)
        {
            _databaseContext.Update(foodPlaceCategory);
            return Save();
        }

        public bool DeleteFoodPlaceCategory(FoodPlaceCategory foodPlaceCategory)
        {
            _databaseContext.Remove(foodPlaceCategory);
            return Save();
        }
    }
}