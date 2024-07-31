using IPSMDB.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IPSMDB.Models
{
    public class FoodPlaceCategory
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<FoodPlace> FoodPlaces { get; set; }
    }
}