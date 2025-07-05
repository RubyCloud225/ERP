using ERP.Model;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] ApplicationDbContext.UserSignUpDto userSignUpDto)
        {
            if (userSignUpDto == null)
            {
                return BadRequest("User data is required.");
            }

            var userId = await _userService.AddUser(userSignUpDto);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Failed to create user.");
            }
            return CreatedAtAction(nameof(GetUserById), new { id = userId }, userId);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var token = await _userService.Login(loginRequest.Username, loginRequest.Password);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Invalid username or password.");
            }
            return Ok(new { Token = token });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID is required.");
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Return user details excluding sensitive information like password
            var userDto = new
            {
                user.Id,
                user.Name,
                user.Username,
                user.Email,
                user.CompanyName,
                user.CountryOfOrigin,
                user.Address,
                user.NumberOfRoles,
                user.CompanyNumber
            };

            return Ok(userDto);
        }
    }

    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
