using AutoMapper;
using IPSMDB.Dto.Models;
using IPSMDB.Models;

namespace IPSMDB.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Person, PersonDto>();
            CreateMap<PersonDto, Person>();

            CreateMap<FoodPlaceCategory, FoodPlaceCategoryDto>();
            CreateMap<FoodPlaceCategoryDto, FoodPlaceCategory>();

            CreateMap<FoodPlace, FoodPlaceDto>();
            CreateMap<FoodPlaceDto, FoodPlace>();

            CreateMap<ServiceReview, ServiceReviewDto>();
            CreateMap<ServiceReviewDto, ServiceReview>();

            CreateMap<Location, LocationDto>();
            CreateMap<LocationDto, Location>();

            CreateMap<CustomerReview, CustomerReviewDto>();
            CreateMap<CustomerReviewDto, CustomerReview>();
        }
    }
}