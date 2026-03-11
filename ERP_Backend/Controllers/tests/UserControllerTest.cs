using ERP.Controllers;
using ERP.Model;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ERP.Controllers.Tests
{
    public class UserControllerTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly UserController _controller;

        public UserControllerTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _controller = new UserController(_mockUserService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Login_ReturnsOk_WithToken_WhenCredentialsValid()
        {
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "password123"
            };

            _mockUserService.Setup(s => s.Login(loginRequest.Username, loginRequest.Password)).ReturnsAsync("jwt-token");

            var result = await _controller.Login(loginRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.Equal("jwt-token", ((dynamic)okResult.Value).Token);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenLoginRequestInvalid()
        {
            var result = await _controller.Login(new LoginRequest
            {
                Username = "testuser",
                Password = "password123"
            });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenLoginFails()
        {
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            _mockUserService.Setup(s => s.Login(loginRequest.Username, loginRequest.Password)).ReturnsAsync((string?)null);

            var result = await _controller.Login(loginRequest);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
