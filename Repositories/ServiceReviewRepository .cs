using IPSMDB.Context;
using IPSMDB.Interfaces;
using IPSMDB.Models;

namespace IPSMDB.Repositories
{
    public class ServiceReviewRepository : IServiceReviewRepository
    {
        private readonly DatabaseContext _databaseContext;
        public ServiceReviewRepository(DatabaseContext context)
        {
            _databaseContext = context;
        }

        public ICollection<ServiceReview> GetServiceReviews()
        {
            return _databaseContext.ServiceReview.OrderBy(fpr => fpr.Id).ToList();
        }

        public bool CreateServiceReview(ServiceReview serviceReview)
        {
            _databaseContext.Add(serviceReview);
            return Save();
        }
        public bool Save()
        {
            var saved = _databaseContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool ServiceReviewExists(int serviceReviewId)
        {
            return _databaseContext.ServiceReview.Any(sr => sr.Id == serviceReviewId);
        }

        public bool UpdateServiceReview(ServiceReview serviceReview)
        {
            _databaseContext.Update(serviceReview);
            return Save();
        }

        public ServiceReview GetServiceReview(int id)
        {
            return _databaseContext.ServiceReview.Where(sr => sr.Id == id).FirstOrDefault();
        }

        public bool DeleteServiceReview(ServiceReview serviceReview)
        {
            _databaseContext.Remove(serviceReview);
            return Save();
        }
    }
}