using AutoMapper;
using IPSMDB.Dto.Models;
using IPSMDB.Interfaces;
using IPSMDB.Models;
using IPSMDB.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IPSMDB.Controllers.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceReviewController : Controller
    {
        private readonly IServiceReviewRepository _serviceReviewRepository;
        private readonly IMapper _mapper;

        public ServiceReviewController(IServiceReviewRepository serviceReviewRepository, IMapper mapper)
        {
            _serviceReviewRepository = serviceReviewRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ServiceReview>))]
        public IActionResult GetServiceReviewss()
        {
            var reviews = _mapper
                .Map<List<ServiceReviewDto>>(_serviceReviewRepository.GetServiceReviews());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateServiceReview([FromBody] ServiceReviewDto createServiceReview)
        {
            if (createServiceReview == null)
                return BadRequest(ModelState);

            var serviceReview = _serviceReviewRepository.GetServiceReviews()
                .Where(fpc => fpc.Id == createServiceReview.Id)
                .FirstOrDefault();

            if (serviceReview != null)
            {
                ModelState.AddModelError("", "Food place already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var serviceReviewMap = _mapper.Map<ServiceReview>(createServiceReview);

            if (!_serviceReviewRepository.CreateServiceReview(serviceReviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created!");
        }

        [HttpPut("{serviceReviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateServiceReview(int serviceReviewId,
            [FromBody] ServiceReviewDto updatedServiceReview)
        {
            if (updatedServiceReview == null)
                return BadRequest(ModelState);

            if (serviceReviewId != updatedServiceReview.Id)
                return BadRequest(ModelState);

            if (!_serviceReviewRepository.ServiceReviewExists(serviceReviewId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var serviceReviewMap = _mapper.Map<ServiceReview>(updatedServiceReview);

            if (!_serviceReviewRepository.UpdateServiceReview(serviceReviewMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{serviceReviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteServiceReview(int serviceReviewId)
        {
            if (!_serviceReviewRepository.ServiceReviewExists(serviceReviewId))
            {
                return NotFound();
            }

            var serviceReviewToDelete = _serviceReviewRepository
                .GetServiceReview(serviceReviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_serviceReviewRepository.DeleteServiceReview(serviceReviewToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}