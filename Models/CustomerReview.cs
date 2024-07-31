using IPSMDB.AbstractClasses;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace IPSMDB.Models
{
    // CustomerReview is used when a person reviews a food place
    public class CustomerReview : Review
    {
        public int ServiceKindness { get; set; }
        public int FoodQuality { get; set; }
    }
}