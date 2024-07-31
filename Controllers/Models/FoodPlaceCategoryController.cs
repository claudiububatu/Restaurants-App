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
    public class FoodPlaceCategoryController : Controller
    {
        private readonly IFoodPlaceCategoryRepository _foodPlaceCategoryRepository;
        private readonly IMapper _mapper;

        public FoodPlaceCategoryController(IFoodPlaceCategoryRepository foodPlaceCategoryRepository, IMapper mapper)
        {
            _foodPlaceCategoryRepository = foodPlaceCategoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<FoodPlaceCategory>))]
        public IActionResult GetFoodCategoryPlaces()
        {
            var foodPlaceCategories = _mapper
                .Map<List<FoodPlaceCategoryDto>>(_foodPlaceCategoryRepository.GetFoodPlaceCategories());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(foodPlaceCategories);
        }

        [HttpGet("{categoryId}/id")]
        [ProducesResponseType(200, Type = typeof(FoodPlaceCategory))]
        [ProducesResponseType(400)]
        public IActionResult GetFoodPlaceCategory(int categoryId)
        {
            if (!_foodPlaceCategoryRepository.FoodPlaceCategoryExists(categoryId))
                return NotFound();

            var foodPlaceCategories = _mapper
                .Map<FoodPlaceCategoryDto>(_foodPlaceCategoryRepository.GetFoodPlaceCategory(categoryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(foodPlaceCategories);
        }

        [HttpGet("{categoryName}/category")]
        [ProducesResponseType(200, Type = typeof(FoodPlaceCategory))]
        [ProducesResponseType(400)]
        public IActionResult GetFoodPlaceCategory(string categoryName)
        {
            if (!_foodPlaceCategoryRepository.FoodPlaceCategoryExists(categoryName))
                return NotFound();

            var foodPlaceCategories = _mapper
                .Map<FoodPlaceCategoryDto>(_foodPlaceCategoryRepository.GetFoodPlaceCategory(categoryName));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(foodPlaceCategories);
        }

        [HttpGet("{categoryName}/foodPlaces")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<FoodPlace>))]
        [ProducesResponseType(400)]
        public IActionResult GetFoodPlaces(string categoryName)
        {
            if (!_foodPlaceCategoryRepository.FoodPlaceCategoryExists(categoryName))
                return NotFound();

            var foodPlaces = _mapper
                .Map<List<FoodPlaceDto>>(_foodPlaceCategoryRepository.GetFoodPlacesByCategory(categoryName));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(foodPlaces);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateFoodPlaceCategory([FromBody] FoodPlaceCategoryDto categoryCreate)
        {
            if (categoryCreate == null)
                return BadRequest(ModelState);

            var foodPlaceCategory = _foodPlaceCategoryRepository.GetFoodPlaceCategories()
                .Where(fpc => fpc.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (foodPlaceCategory != null)
            {
                ModelState.AddModelError("", "Food place category already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var foodPlaceCategoryMap = _mapper.Map<FoodPlaceCategory>(categoryCreate);

            if (!_foodPlaceCategoryRepository.CreateFoodPlaceCategory(foodPlaceCategoryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created!");
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateFoodPlaceCategory(int categoryId,
            [FromBody] FoodPlaceCategoryDto updatedCategory)
        {
            if (updatedCategory == null)
                return BadRequest(ModelState);

            if (categoryId != updatedCategory.Id)
                return BadRequest(ModelState);

            if (!_foodPlaceCategoryRepository.FoodPlaceCategoryExists(categoryId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var categoryMap = _mapper.Map<FoodPlaceCategory>(updatedCategory);

            if (!_foodPlaceCategoryRepository.UpdateFoodPlaceCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{foodPlaceCategoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteFoodPlaceCategory(int foodPlaceCategoryId)
        {
            if (!_foodPlaceCategoryRepository.FoodPlaceCategoryExists(foodPlaceCategoryId))
            {
                return NotFound();
            }

            var categoryToDelete = _foodPlaceCategoryRepository
                .GetFoodPlaceCategory(foodPlaceCategoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_foodPlaceCategoryRepository.DeleteFoodPlaceCategory(categoryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}