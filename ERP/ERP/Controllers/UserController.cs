using Azure.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        // add new user
        [HttpPost("add")]
        public async Task<IActionResult> AddNewUser(User user) // error will be resolved by the user service
        {
            if(Username != null)
            {
                return Bad
            }
        }
    }
}