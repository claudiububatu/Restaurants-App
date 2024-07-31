using AutoMapper;
using IPSMDB.Dto.Models;
using IPSMDB.Interfaces;
using IPSMDB.Models;
using IPSMDB.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace IPSMDB.Controllers.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : Controller
    {
        private readonly IPersonRepository _personRepository;

        private readonly IMapper _mapper;

        public PersonController(IPersonRepository personRepository,
            IMapper mapper)
        {
            _personRepository = personRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Person>))]
        public IActionResult GetPerson()
        {
            var people = _mapper.Map<List<PersonDto>>(_personRepository.GetPerson());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(people);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePerson([FromBody] PersonDto personCreate)
        {
            if (personCreate == null)
                return BadRequest(ModelState);

            var person = _personRepository.GetPerson()
                .Where(p => p.LastName.Trim().ToUpper() == personCreate.LastName.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (person != null)
            {
                ModelState.AddModelError("", "Food place already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var personMap = _mapper.Map<Person>(personCreate);

            if (!_personRepository.CreatePerson(personMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created!");
        }

        [HttpGet("{firstName}/{lastName}/recommendations")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<FoodPlaceDto>))]
        [ProducesResponseType(404)]
        public IActionResult GetRecommendationsFromFriends(string firstName, string lastName)
        {
            if (!_personRepository.PersonExists(lastName, firstName))
            {
                Console.WriteLine($"User {firstName} {lastName} not found.");
                return NotFound();
            }

            var person = _personRepository.GetPerson(lastName, firstName);
            if (person == null)
            {
                Console.WriteLine($"Person object for {firstName} {lastName} is null.");
                return NotFound();
            }

            var recommendations = _personRepository.GetFriendsNewFoodPlaces(firstName, lastName);
            if (recommendations == null || !recommendations.Any())
            {
                Console.WriteLine($"No recommendations found for {firstName} {lastName}.");
                return NotFound();
            }

            Console.WriteLine($"Found {recommendations.Count()} recommendations for {firstName} {lastName}.");

            var userFavCategories = _personRepository.GetFavoriteFoodCategoryIds(person.Id)?.ToList();
            if (userFavCategories == null || !userFavCategories.Any())
            {
                Console.WriteLine($"No favorite categories found for user {firstName} {lastName}.");
                return NotFound();
            }

            Console.WriteLine($"User {firstName} {lastName} has {userFavCategories.Count} favorite categories.");

            var sortedRecommendations = new List<FoodPlace>();

            foreach (var categoryId in userFavCategories)
            {
                var categoryRecommendations = recommendations
                    .Where(r => r.Id == categoryId)
                    .ToList();

                if (categoryRecommendations.Any())
                {
                    Console.WriteLine($"Found {categoryRecommendations.Count} recommendations for category {categoryId}.");
                }

                sortedRecommendations = categoryRecommendations;
            }

            if (!sortedRecommendations.Any())
            {
                Console.WriteLine($"No sorted recommendations after filtering for user {firstName} {lastName}.");
                sortedRecommendations = recommendations
                    .ToList();
            }

            var smallRecList = sortedRecommendations.Count > 3 ? sortedRecommendations.Take(3).ToList() : sortedRecommendations;

            var smallRecListDto = _mapper.Map<List<FoodPlaceDto>>(smallRecList);

            return Ok(smallRecListDto);
        }

        [HttpGet("name/{name}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Person>))]
        [ProducesResponseType(400)]
        public IActionResult GetPersonsByNameMatch(string name)
        {
            if (!_personRepository.PersonExists(name))
                return NotFound();

            var people = _mapper.Map<List<PersonDto>>(_personRepository.GetPersonsByNameMatch(name));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(people);
        }

        [HttpGet("{personId}")]
        [ProducesResponseType(200, Type = typeof(Person))]
        [ProducesResponseType(400)]
        public IActionResult GetPerson(int personId)
        {
            if (!_personRepository.PersonExists(personId))
                return NotFound();

            var person = _mapper.Map<PersonDto>(_personRepository.GetPerson(personId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(person);
        }

        [HttpGet("{lastName}/{firstName}")]
        [ProducesResponseType(200, Type = typeof(Person))]
        [ProducesResponseType(400)]
        public IActionResult GetPerson(string lastName, string firstName)
        {
            if (!_personRepository.PersonExists(lastName, firstName))
                return NotFound();

            var person = _mapper.Map<PersonDto>(_personRepository.GetPerson(lastName, firstName));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(person);
        }


        [HttpGet("{personId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPersonIdRating(int personId)
        {
            if (!_personRepository.PersonExists(personId))
                return NotFound();

            var rating = _personRepository.GetPersonRating(personId);

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(rating);
        }

        [HttpGet("{name}/nameRating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPersonNameRating(string name)
        {
            if (!_personRepository.PersonExists(name))
                return NotFound();

            var rating = _personRepository.GetPersonRating(name);

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(rating);
        }

        [HttpGet("{firstName}/{lastName}/foodPlaces")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<FoodPlace>))]
        [ProducesResponseType(400)]
        public IActionResult GetFoodPlaces(string firstName, string lastName)
        {
            if (!_personRepository.PersonExists(lastName, firstName))
                return NotFound();

            var person = _personRepository.GetPerson(lastName, firstName);

            var foodPlaces = _mapper.Map<List<FoodPlaceDto>>(_personRepository.GetFoodPlacesById(person.Id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(foodPlaces);
        }

        [HttpGet("{personId}/foodPlaces")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<FoodPlace>))]
        [ProducesResponseType(400)]
        public IActionResult GetFoodPlaces(int personId)
        {
            if (!_personRepository.PersonExists(personId))
                return NotFound();

            var foodPlaces = _mapper.Map<List<FoodPlaceDto>>(_personRepository.GetFoodPlacesById(personId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(foodPlaces);
        }

        [HttpPost("{personId}/friends/{friendId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateFriendship(int personId, int friendId)
        {
            if (!_personRepository.PersonExists(personId) || !_personRepository.PersonExists(friendId))
            {
                return NotFound();
            }

            if (!_personRepository.CreateFriendship(personId, friendId))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created friendship!");
        }

        [HttpGet("{firstName}/{lastName}/friends")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<PersonDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetFriendships(string firstName, string lastName)
        {
            if (!_personRepository.PersonExists(lastName, firstName))
            {
                return NotFound();
            }

            var friends = _personRepository.GetFriends(firstName, lastName);

            var friendsMap = _mapper.Map<List<PersonDto>>(friends);

            return Ok(friendsMap);
        }

        [HttpPut("{personId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePerson(int personId,
            [FromBody] PersonDto updatedPerson)
        {
            if (updatedPerson == null)
                return BadRequest(ModelState);

            if (personId != updatedPerson.Id)
                return BadRequest(ModelState);

            if (!_personRepository.PersonExists(personId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var personMap = _mapper.Map<Person>(updatedPerson);

            if (!_personRepository.UpdatePerson(personMap))
            {
                ModelState.AddModelError("", "Something went wrong updating person!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{personId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePerson(int personId)
        {
            if (!_personRepository.PersonExists(personId))
            {
                return NotFound();
            }

            var personToDelete = _personRepository
                .GetPerson(personId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_personRepository.DeletePerson(personToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting location!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}