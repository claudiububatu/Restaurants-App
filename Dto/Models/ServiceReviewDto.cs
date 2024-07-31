namespace IPSMDB.Dto.Models
{
    public class ServiceReviewDto
    {
        public int Id { get; set; }
        public int CustomerKindness { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int FoodPlaceId { get; set; }
        public int PersonId { get; set; }
    }
}