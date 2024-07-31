using AutoMapper;
using IPSMDB.Context;
using IPSMDB.Dto.Models;
using IPSMDB.Interfaces;
using IPSMDB.Models;
using IPSMDB.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace IPSMDB.Controllers.Models
{
    public class UpdateImageRequest
    {
        public string ImageUrl { get; set; }
        public string FoodPlaceName { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class FoodPlaceController : Controller
    {
        private readonly IFoodPlaceRepository _foodPlaceRepository;
        private readonly IFoodPlaceCategoryRepository _foodPlaceCategoryRepository;
        private readonly IPersonRepository _personRepository;
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public FoodPlaceController(IFoodPlaceRepository foodPlaceRepository,
            IFoodPlaceCategoryRepository foodPlaceCategoryRepository,
            IPersonRepository personRepository,
            DatabaseContext databaseContext,
            IMapper mapper)
        {
            _foodPlaceRepository = foodPlaceRepository;
            _foodPlaceCategoryRepository = foodPlaceCategoryRepository;
            _personRepository = personRepository;
            _databaseContext = databaseContext;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<FoodPlace>))]
        public IActionResult GetFoodPlacess()
        {
            var foodPlaces = _mapper.Map<List<FoodPlaceDto>>(_foodPlaceRepository.GetFoodPlaces());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(foodPlaces);
        }

        [HttpGet("{foodPlaceId}/id")]
        [ProducesResponseType(200, Type = typeof(FoodPlace))]
        [ProducesResponseType(400)]
        public IActionResult GetFoodPlace(int foodPlaceId)
        {
            if (!_foodPlaceRepository.FoodPlaceExists(foodPlaceId))
                return NotFound();

            var foodPlace = _mapper
                .Map<FoodPlaceDto>(_foodPlaceRepository.GetFoodPlace(foodPlaceId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(foodPlace);
        }

        [HttpGet("{foodPlaceName}/name")]
        [ProducesResponseType(200, Type = typeof(FoodPlace))]
        [ProducesResponseType(400)]
        public IActionResult GetFoodPlace(string foodPlaceName)
        {
            if (!_foodPlaceRepository.FoodPlaceExists(foodPlaceName))
                return NotFound();

            var foodPlace = _mapper
                .Map<FoodPlaceDto>(_foodPlaceRepository.GetFoodPlace(foodPlaceName));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(foodPlace);
        }

        [HttpGet("{foodPlaceName}/locations")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Location>))]
        [ProducesResponseType(400)]
        public IActionResult GetLocations(string foodPlaceName)
        {
            if (!_foodPlaceRepository.FoodPlaceExists(foodPlaceName))
                return NotFound();

            var locations = _mapper
                .Map<List<LocationDto>>(_foodPlaceRepository.GetLocationsByFoodPlaceName(foodPlaceName));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(locations);
        }

        [HttpGet("{foodPlaceName}/reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CustomerReview>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviews(string foodPlaceName)
        {
            if (!_foodPlaceRepository.FoodPlaceExists(foodPlaceName))
                return NotFound();

            var reviews = _mapper
                .Map<List<CustomerReviewDto>>(_foodPlaceRepository.GetPersonReviewsForFoodPlaceName(foodPlaceName));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [Authorize]
        public IActionResult CreateFoodPlace([FromQuery] int foodPlaceCategoryId,
            [FromBody] FoodPlaceDto foodPlaceCreate)
        {
            if (foodPlaceCreate == null)
                return BadRequest(ModelState);

            var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var firstName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;

            var lastName = User.Claims.FirstOrDefault(c => c.Type == "lastName")?.Value;

            var id = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            if (firstName == null || lastName == null)
                return Unauthorized();

            var person = _personRepository.GetPerson(lastName, firstName);

            if (person == null)
                return NotFound("Person not found!");

            var foodPlace = _foodPlaceRepository.GetFoodPlaces()
                .Where(fpc => fpc.Name.Trim().ToUpper() == foodPlaceCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (foodPlace != null && foodPlace.PersonId == person.Id)
            {
                ModelState.AddModelError("", "Food place already exists for current user!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var foodPlaceMap = _mapper.Map<FoodPlace>(foodPlaceCreate);
            foodPlaceMap.FoodPlaceCategory = _foodPlaceCategoryRepository
                .GetFoodPlaceCategory(foodPlaceCategoryId);

            foodPlaceMap.PersonId = person.Id;

            if (!_foodPlaceRepository.CreateFoodPlace(foodPlaceMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created!");
        }

        [HttpPut("{foodPlaceId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateFoodPlace(int foodPlaceId,
            [FromBody] FoodPlaceDto updatedFoodPlace)
        {
            if (updatedFoodPlace == null)
                return BadRequest(ModelState);

            if (foodPlaceId != updatedFoodPlace.Id)
                return BadRequest(ModelState);

            if (!_foodPlaceRepository.FoodPlaceExists(foodPlaceId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var foodPlaceMap = _mapper.Map<FoodPlace>(updatedFoodPlace);

            if (!_foodPlaceRepository.UpdateFoodPlace(foodPlaceMap))
            {
                ModelState.AddModelError("", "Something went wrong updating food place!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{foodPlaceId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteFoodPlace(int foodPlaceId)
        {
            if (!_foodPlaceRepository.FoodPlaceExists(foodPlaceId))
            {
                return NotFound();
            }

            var foodPlaceToDelete = _foodPlaceRepository
                .GetFoodPlace(foodPlaceId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_foodPlaceRepository.DeleteFoodPlace(foodPlaceToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateImage")]
        public IActionResult UpdateImage([FromBody] UpdateImageRequest request)
        {
            if (!_foodPlaceRepository.FoodPlaceExists(request.FoodPlaceName))
                return NotFound();

            var firstName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            var lastName = User.Claims.FirstOrDefault(c => c.Type == "lastName")?.Value;

            var person = _personRepository.GetPerson(lastName, firstName);

            if (person == null)
                return NotFound("Person not found!");

            Console.WriteLine(person.Id);

            var foodPlaces = _foodPlaceRepository.GetFoodPlaces();

            // Găsim toate FoodPlace-urile care au același nume și aparțin persoanei autentificate
            var foodPlacesToUpdate = foodPlaces.Where(fp => fp.Name == request.FoodPlaceName).ToList();

            if (!foodPlacesToUpdate.Any())
                return NotFound("No matching food places found for the authenticated person.");

            foreach (var foodPlace in foodPlacesToUpdate)
            {
                foodPlace.ImageUrl = request.ImageUrl;
                _databaseContext.Update(foodPlace);
            }

            _databaseContext.SaveChanges();

            return Ok(foodPlacesToUpdate);
        }

    }
}