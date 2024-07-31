using AutoMapper;
using IPSMDB.Dto.Models;
using IPSMDB.Interfaces;
using IPSMDB.Models;
using IPSMDB.Repositories;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace IPSMDB.Controllers.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : Controller
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IFoodPlaceRepository _foodPlaceRepository;
        private readonly IMapper _mapper;

        public LocationController(ILocationRepository locationRepository, IFoodPlaceRepository foodPlaceRepository, IMapper mapper)
        {
            _locationRepository = locationRepository ?? throw new ArgumentNullException(nameof(locationRepository));
            _foodPlaceRepository = foodPlaceRepository ?? throw new ArgumentNullException(nameof(foodPlaceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{foodPlaceName}/locations")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<LocationDto>))]
        public IActionResult GetLocationsByFoodPlace(int foodPlaceName)
        {
            var locations = _mapper.Map<List<LocationDto>>(_locationRepository.GetLocation(foodPlaceName));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(locations);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Location>))]
        public IActionResult GetLocation()
        {
            var locations = _mapper.Map<List<LocationDto>>(_locationRepository.GetLocations());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(locations);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateLocation([FromBody] LocationDto createLocation)
        {
            if (createLocation == null)
                return BadRequest("Location data is null.");

            // Check if the FoodPlaceId exists in the FoodPlace table
            Console.WriteLine(createLocation.FoodPlaceId);
            Console.WriteLine("AICIIIIIIII");

            var foodPlace = _foodPlaceRepository.GetFoodPlace(createLocation.FoodPlaceId);
            if (foodPlace == null)
            {
                ModelState.AddModelError("FoodPlaceId", "The specified FoodPlaceId does not exist.");
                return BadRequest(ModelState);
            }

            // Check if a location with the same Id already exists
            var existingLocation = _locationRepository.GetLocations()
                .FirstOrDefault(loc => loc.Id == createLocation.Id);

            if (existingLocation != null)
            {
                ModelState.AddModelError("", "Location already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var locationMap = _mapper.Map<Location>(createLocation);

            if (!_locationRepository.CreateLocation(locationMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created!");
        }


        [HttpPut("{locationId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateLocation(int locationId,
            [FromBody] LocationDto updatedLocation)
        {
            if (updatedLocation == null)
                return BadRequest(ModelState);

            if (locationId != updatedLocation.Id)
                return BadRequest(ModelState);

            if (!_locationRepository.LocationExists(locationId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var locationMap = _mapper.Map<Location>(updatedLocation);

            if (!_locationRepository.UpdateLocation(locationMap))
            {
                ModelState.AddModelError("", "Something went wrong updating location!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{locationId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteLocation(int locationId)
        {
            if (!_locationRepository.LocationExists(locationId))
            {
                return NotFound();
            }

            var locationToDelete = _locationRepository
                .GetLocation(locationId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_locationRepository.DeleteLocation(locationToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting location!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}