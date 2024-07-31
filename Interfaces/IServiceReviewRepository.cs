using IPSMDB.Models;

namespace IPSMDB.Interfaces
{
    public interface IServiceReviewRepository
    {
        ICollection<ServiceReview> GetServiceReviews();
        ServiceReview GetServiceReview(int id);
        bool ServiceReviewExists(int serviceReviewId);
        bool CreateServiceReview(ServiceReview serviceReview);
        bool UpdateServiceReview(ServiceReview serviceReview);
        bool DeleteServiceReview(ServiceReview serviceReview);
        bool Save();
    }
}