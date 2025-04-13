using Azure.Identity;
using ERP.Model;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _dbContext;
        public UserController(ILogger<UserController> logger, IUserService userService, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _userService = userService;
            _dbContext = dbContext;
        }
        // add new user
        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] ApplicationDbContext.UserDto userDto) // error will be resolved by the user service
        {
            if(!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid User Data provided");
                return BadRequest(ModelState);
            }

            var userId = await _userService.AddUser(userDto);
            if (userId == null)
            {
                _logger.LogWarning("Failed to add user");
                return StatusCode(500, "Failed to add user");
           }
            _logger.LogInformation("User added successfully");
            return Ok(userId);
        }
        [HttpPost("login ")]
        public async Task<IActionResult> Login([FromBody] ApplicationDbContext.loginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid Login Data provided");
                return BadRequest(ModelState);
            }
            var token = await _userService.Login(loginDto.Username, loginDto.Password);
            if (token == null)
            {
                _logger.LogWarning("Failed to login user");
                return Unauthorized();
            }
            _logger.LogInformation("User logged in successfully");
            return Ok(token);
        }
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser (string userId, [FromBody]ApplicationDbContext.UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid User Data provided");
                return BadRequest(ModelState);
            }
            var result = await _userService.UpdateUser(userId, userDto);
            if (!result)
            {
                _logger.LogWarning("Failed to update user");
                return NotFound();
            }
            _logger.LogInformation("User updated successfully");
            return Ok();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _userService.DeleteUser(userId);
            if (!result)
            {
                _logger.LogWarning("Failed to delete user");
                return NotFound();
            }
            _logger.LogInformation("User deleted successfully");
            return Ok();
        }
    }
}