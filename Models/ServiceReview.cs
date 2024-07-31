using IPSMDB.AbstractClasses;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace IPSMDB.Models
{
    // ServiceReview is used when a food place reviews a person
    public class ServiceReview : Review
    {
        public int CustomerKindness { get; set; }
    }
}