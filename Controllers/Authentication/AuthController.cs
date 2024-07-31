using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using IPSMDB.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IPSMDB.Dto.Authentication;
using IPSMDB.Context;
using IPSMDB.Interfaces;
using Microsoft.AspNetCore.Authorization;
using IPSMDB.Dto.Models;
using AutoMapper;

namespace IPSMDB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly DatabaseContext _databaseContext;
        private readonly IPersonRepository _personRepository;
        private readonly IFoodPlaceRepository _foodPlaceRepository;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IMapper _mapper;

        public AuthController(UserManager<AppUser> userManager,
            IConfiguration configuration,
            DatabaseContext databaseContext,
            IPersonRepository personRepository,
            IFoodPlaceRepository foodPlaceRepository,
            IAppUserRepository appUserRepository,
            IMapper mapper)
        {
            _userManager = userManager;
            _configuration = configuration;
            _databaseContext = databaseContext;
            _personRepository = personRepository;
            _foodPlaceRepository = foodPlaceRepository;
            _appUserRepository = appUserRepository;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Ok(new { token = GenerateJwtToken(user) });
            }

            return Unauthorized();
        }


        private string GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("id", user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble("365"));

            var token = new JwtSecurityToken(
                "http://localhost:44304",
                "http://localhost:44304",
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new AppUser
            {
                UserName = model.Username,
                Email = model.Email,
                Role = "User",
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            var person = _personRepository.GetPerson()
                .Where(p => p.LastName.Trim().ToUpper() == model.LastName.TrimEnd().ToUpper()
                    && p.FirstName.Trim().ToUpper() == model.FirstName.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (person != null)
            {
                person.EmailAddress = model.Email;
                _databaseContext.Person.Update(person);
                await _databaseContext.SaveChangesAsync();

                user.PersonId = person.Id;
            }
            else
            {
                person = new Person
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.Email,
                };

                _databaseContext.Person.Add(person);
                await _databaseContext.SaveChangesAsync();

                user.PersonId = person.Id;
            }

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { token = GenerateJwtToken(user) });
            }

            return BadRequest(result.Errors);
        }


        [HttpGet("getAllUsers")]
        /*[Authorize(Roles = "Admin")]*/
        public async Task<IActionResult> GetAllUsers()
        {
            // Presupunem că ai un serviciu UserService care se ocupă de operațiunile legate de utilizatori
            var users = _appUserRepository.GetAppUsers();

            // Convertim lista de utilizatori într-un format adecvat pentru răspuns
            var userDtos = users.Select(user => new AppUser
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PersonId = user.PersonId,
            });

            return Ok(userDtos);
        }

        [HttpPost("makeAdmin/{userId}")]
        /*[Authorize(Roles = "Admin")]*/
        public async Task<IActionResult> MakeAdmin(int userId)
        {
            var user = _appUserRepository.GetAppUser(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (user.Role == "Admin")
            {
                return BadRequest("User is already an admin");
            }

            user.Role = "Admin";
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to update user role");
            }

            await _databaseContext.SaveChangesAsync();

            return Ok("User role updated to admin");
        }

        [HttpGet("getProfile/{firstName}/{lastName}")]
        public async Task<IActionResult> getProfile(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return BadRequest("First name and last name cannot be null or empty.");
            }

            var users = _appUserRepository.GetAppUsers();

            // Căutăm utilizatorul după nume și prenume
            var user = users.FirstOrDefault(u => u.FirstName == firstName && u.LastName == lastName);

            // Dacă nu am găsit utilizatorul, returnăm un mesaj de eroare
            if (user == null)
            {
                return NotFound($"Nu a fost găsit niciun utilizator cu numele {firstName} {lastName}.");
            }

            // Convertim utilizatorul într-un format adecvat pentru răspuns
            var userDto = new AppUser
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PersonId = user.PersonId,
            };

            return Ok(userDto);
        }


        [HttpGet("getFoodPlaces")]
        /*[Authorize]*/
        public IActionResult GetFoodPlaces()
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var firstName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;

            var lastName = User.Claims.FirstOrDefault(c => c.Type == "lastName")?.Value;

            if (userName == null)
                return Unauthorized();

            if (firstName == null || lastName == null)
                return Unauthorized();

            var person = _personRepository.GetPerson(lastName, firstName);

            if (person == null)
                return NotFound("Person not found");

            var foodPlaces = _mapper.Map<List<FoodPlaceDto>>(_personRepository.GetFoodPlacesById(person.Id));

            return Ok(foodPlaces);
        }

        [HttpGet("getProfile")]
        /*[Authorize]*/
        public async Task<IActionResult> GetProfile()
        {
            // Obținem numele de utilizator din claim-urile token-ului JWT
            var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userName == null)
                return Unauthorized();

            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound();

            var userDto = new AppUser
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PersonId = user.PersonId,
            };

            return Ok(userDto);
        }
    }
}