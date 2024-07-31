using System.ComponentModel.DataAnnotations.Schema;

namespace IPSMDB.Dto.Models
{
    public class FoodPlaceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FoodPlaceCategoryId { get; set; }
        public int PersonId { get; set; }
        public string? ImageUrl { get; set; }
    }
}