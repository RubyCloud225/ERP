using Microsoft.AspNetCore.Mvc;
using ERP.Service;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using System.Collections.Generic;
using System;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _userService.Login(request.Username, request.Password);
            if (token == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            return Ok(new { token });
        }

        [HttpGet("oauth-login")]
        public IActionResult OAuthLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("OAuthCallback") };
            return Challenge(properties, "OAuthProvider");
        }

        [HttpGet("oauth-callback")]
        public async Task<IActionResult> OAuthCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("External");
            if (!authenticateResult.Succeeded)
            {
                return Unauthorized();
            }

            var externalUser = authenticateResult.Principal;
            var email = externalUser.FindFirst(ClaimTypes.Email)?.Value;
            var name = externalUser.FindFirst(ClaimTypes.Name)?.Value;

            // TODO: Find or create user in database based on external login info
            // For now, just log and return a dummy token

            _logger.LogInformation("OAuth login successful for {Email}", email);

            // Generate JWT token for the user (implement accordingly)
            var token = "dummy-jwt-token";

            return Ok(new { token });
        }
    }

    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
