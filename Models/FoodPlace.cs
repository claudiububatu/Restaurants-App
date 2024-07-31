
using IPSMDB.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPSMDB.Models
{
    public class FoodPlace
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("FoodPlaceCategory")]
        public int FoodPlaceCategoryId { get; set; }
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public FoodPlaceCategory FoodPlaceCategory { get; set; }
        public ICollection<Location> Locations { get; set; }
        public ICollection<CustomerReview> ReviewsReceived { get; set; }
        public ICollection<ServiceReview> ReviewsWritten { get; set; }
        public string? ImageUrl { get; set; }
    }
}