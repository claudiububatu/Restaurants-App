using IPSMDB.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPSMDB.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        [ForeignKey("FoodPlace")]
        public int FoodPlaceId { get; set; }
        public FoodPlace FoodPlace { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
