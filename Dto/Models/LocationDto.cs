namespace IPSMDB.Dto.Models
{
    public class LocationDto
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int FoodPlaceId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}