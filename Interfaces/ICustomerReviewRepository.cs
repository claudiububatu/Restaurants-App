using IPSMDB.Models;

namespace IPSMDB.Interfaces
{
    public interface ICustomerReviewRepository
    {
        ICollection<CustomerReview> GetCustomerReviews();
        CustomerReview GetCustomerReview(int id);
        bool CustomerReviewExists(int customerReviewId);
        bool CreateCustomerReview(CustomerReview customerReview);
        bool UpdateCustomerReview(CustomerReview customerReview);
        bool DeleteCustomerReview(CustomerReview customerReview);
        bool Save();
        ICollection<CustomerReview> GetReviewsByFoodPlaceId(int foodPlaceId);
        ICollection<CustomerReview> GetReviewsByFoodPlaceName(string foodPlaceName);
        ICollection<CustomerReview> GetReviewsByPersonId(int personId);

    }
}