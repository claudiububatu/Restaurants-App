using AutoMapper;
using IPSMDB.Dto.Models;
using IPSMDB.Interfaces;
using IPSMDB.Models;
using IPSMDB.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace IPSMDB.Controllers.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerReviewController : Controller
    {
        private readonly ICustomerReviewRepository _customerReviewRepository;
        private readonly IMapper _mapper;

        public CustomerReviewController(ICustomerReviewRepository customerReviewRepository, IMapper mapper)
        {
            _customerReviewRepository = customerReviewRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CustomerReview>))]
        public IActionResult GetPersonReviews()
        {
            var personReviews = _mapper
                .Map<List<CustomerReviewDto>>(_customerReviewRepository.GetCustomerReviews());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(personReviews);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCustomerReview([FromBody] CustomerReviewDto createCustomerReview)
        {
            if (createCustomerReview == null)
                return BadRequest(ModelState);

            var customerReview = _customerReviewRepository.GetCustomerReviews()
                .Where(fpc => fpc.Id == createCustomerReview.Id)
                .FirstOrDefault();

            if (customerReview != null)
            {
                ModelState.AddModelError("", "Food place already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customerReviewMap = _mapper.Map<CustomerReview>(createCustomerReview);

            if (!_customerReviewRepository.CreateCustomerReview(customerReviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created!");
        }

        [HttpPut("{customerReviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCustomerReview(int customerReviewId,
            [FromBody] CustomerReviewDto updatedCustomerReview)
        {
            if (updatedCustomerReview == null)
                return BadRequest(ModelState);

            if (customerReviewId != updatedCustomerReview.Id)
                return BadRequest(ModelState);

            if (!_customerReviewRepository.CustomerReviewExists(customerReviewId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var customerReviewMap = _mapper.Map<CustomerReview>(updatedCustomerReview);

            if (!_customerReviewRepository.UpdateCustomerReview(customerReviewMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{customerReviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCustomerReview(int customerReviewId)
        {
            if (!_customerReviewRepository.CustomerReviewExists(customerReviewId))
            {
                return NotFound();
            }

            var categoryToDelete = _customerReviewRepository
                .GetCustomerReview(customerReviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_customerReviewRepository.DeleteCustomerReview(categoryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpGet("foodplace/{foodPlaceId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CustomerReview>))]
        public IActionResult GetReviewsByFoodPlaceId(int foodPlaceId)
        {
            var reviews = _mapper
                .Map<List<CustomerReviewDto>>(_customerReviewRepository.GetReviewsByFoodPlaceId(foodPlaceId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }

        [HttpGet("foodPlace/{foodPlaceId}/numberOfReviews")]
        [ProducesResponseType(200, Type = typeof(int))]
        public IActionResult GetNumberOfReviewsPerRestaurant(int foodPlaceId)
        {
            var reviews = _mapper
                .Map<List<CustomerReviewDto>>(_customerReviewRepository.GetReviewsByFoodPlaceId(foodPlaceId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews.Count);
        }

        [HttpGet("foodQuality/{foodPlaceId}")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        public IActionResult GetFoodQuality(int foodPlaceId)
        {
            var reviews = _mapper.Map<List<CustomerReviewDto>>(_customerReviewRepository.GetReviewsByFoodPlaceId(foodPlaceId));

            double foodQuality = 0;
            int cnt = 0;

            foreach (var review in reviews)
            {
                foodQuality += review.FoodQuality;
                cnt++;
            }

            double foodQualityMA = foodQuality / cnt;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(foodQualityMA);
        }

        [HttpGet("foodQualityByName/{foodPlaceName}")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        public IActionResult GetFoodQualityByName(string foodPlaceName)
        {
            var reviews = _mapper.Map<List<CustomerReviewDto>>(_customerReviewRepository.GetReviewsByFoodPlaceName(foodPlaceName));

            double foodQuality = 0;
            int cnt = 0;

            foreach (var review in reviews)
            {
                foodQuality += review.FoodQuality;
                cnt++;
            }

            double foodQualityMA = foodQuality / cnt;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(foodQualityMA);
        }

        [HttpGet("serviceKindnessByName/{foodPlaceName}")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        public IActionResult GetServiceKindness(string foodPlaceName)
        {
            var reviews = _mapper.Map<List<CustomerReviewDto>>(_customerReviewRepository.GetReviewsByFoodPlaceName(foodPlaceName));

            double serviceKindness = 0;
            int cnt = 0;

            foreach (var review in reviews)
            {
                serviceKindness += review.ServiceKindness;
                cnt++;
            }

            double serviceKindnessMA = serviceKindness / cnt;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(serviceKindnessMA);
        }

        [HttpGet("serviceKindness/{foodPlaceId}")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        public IActionResult GetServiceKindness(int foodPlaceId)
        {
            var reviews = _mapper.Map<List<CustomerReviewDto>>(_customerReviewRepository.GetReviewsByFoodPlaceId(foodPlaceId));

            double serviceKindness = 0;
            int cnt = 0;

            foreach (var review in reviews)
            {
                serviceKindness += review.ServiceKindness;
                cnt++;
            }

            double serviceKindnessMA = serviceKindness / cnt;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(serviceKindnessMA);
        }

        [HttpGet("person/{personId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CustomerReview>))]
        public IActionResult GetReviewsByPersonId(int personId)
        {
            var reviews = _mapper
                .Map<List<CustomerReviewDto>>(_customerReviewRepository.GetReviewsByPersonId(personId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }

        [HttpGet("person/{personId}/numberOfReviews")]
        [ProducesResponseType(200, Type = typeof(int))]
        public IActionResult GetNumberOfReviewsPerPerson(int personId)
        {
            var reviews = _mapper
                .Map<List<CustomerReviewDto>>(_customerReviewRepository.GetReviewsByPersonId(personId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews.Count);
        }

    }
}