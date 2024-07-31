namespace IPSMDB.Dto.Models
{
    public class CustomerReviewDto
    {
        public int Id { get; set; }
        public int ServiceKindness { get; set; }
        public int FoodQuality { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int PersonId { get; set; }
        public int FoodPlaceId { get; set; }
    }
}