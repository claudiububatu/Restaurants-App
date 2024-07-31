using IPSMDB.Context;
using IPSMDB.Models;
using IPSMDB.Interfaces;
using IPSMDB.Context;
using IPSMDB.Interfaces;
using IPSMDB.Models;

namespace IPSMDB.Repositories
{
    public class CustomerReviewRepository : ICustomerReviewRepository
    {
        private readonly DatabaseContext _databaseContext;
        public CustomerReviewRepository(DatabaseContext context)
        {
            _databaseContext = context;
        }

        public bool CreateCustomerReview(CustomerReview customerReview)
        {
            _databaseContext.Add(customerReview);
            return Save();
        }

        public ICollection<CustomerReview> GetCustomerReviews()
        {
            return _databaseContext.CustomerReview.OrderBy(p => p.Id).ToList();
        }

        public bool Save()
        {
            var saved = _databaseContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateCustomerReview(CustomerReview customerReview)
        {
            _databaseContext.Update(customerReview);
            return Save();
        }

        public bool CustomerReviewExists(int customerReviewId)
        {
            return _databaseContext.CustomerReview.Any(cr => cr.Id == customerReviewId);
        }

        public bool DeleteCustomerReview(CustomerReview customerReview)
        {
            _databaseContext.Remove(customerReview);
            return Save();
        }

        public CustomerReview GetCustomerReview(int id)
        {
            return _databaseContext.CustomerReview.Where(cr => cr.Id == id).FirstOrDefault();
        }

        public ICollection<CustomerReview> GetReviewsByFoodPlaceId(int foodPlaceId)
        {
            return _databaseContext.CustomerReview
                .Where(cr => cr.FoodPlaceId == foodPlaceId)
                .OrderByDescending(cr => cr.Id)
                .ToList();
        }

        public ICollection<CustomerReview> GetReviewsByFoodPlaceName(string foodPlaceName)
        {
            var foodPlace = _databaseContext.FoodPlace
                     .Where(fp => fp.Name == foodPlaceName && fp.PersonId == 5)
                     .FirstOrDefault();


            return _databaseContext.CustomerReview
                .Where(cr => cr.FoodPlaceId == foodPlace.Id)
                .OrderByDescending(cr => cr.Id)
                .ToList();
        }

        public ICollection<CustomerReview> GetReviewsByPersonId(int personId)
        {
            return _databaseContext.CustomerReview
                .Where(cr => cr.PersonId == personId)
                .OrderByDescending(cr => cr.Id)
                .ToList();
        }
    }
}