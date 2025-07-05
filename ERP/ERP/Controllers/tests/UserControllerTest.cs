using ERP.Controllers;
using ERP.Model;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ERP.Controllers.Tests
{
    public class UserControllerTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTest()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);
        }

        [Fact]
        public async Task SignUp_ReturnsCreatedAtAction_WhenUserCreated()
        {
            var userSignUpDto = new ApplicationDbContext.UserSignUpDto
            {
                Name = "Test User",
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "password123",
                CompanyName = "Test Company",
                CountryOfOrigin = "Test Country",
                Address = "123 Test St",
                NumberOfRoles = 2,
                CompanyNumber = "123456789"
            };

            _mockUserService.Setup(s => s.AddUser(userSignUpDto)).ReturnsAsync("new-user-id");

            var result = await _controller.SignUp(userSignUpDto);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("new-user-id", createdAtActionResult.Value);
        }

        [Fact]
        public async Task SignUp_ReturnsBadRequest_WhenUserDataIsNull()
        {
            var result = await _controller.SignUp(new ApplicationDbContext.UserSignUpDto());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SignUp_ReturnsBadRequest_WhenUserCreationFails()
        {
            var userSignUpDto = new ApplicationDbContext.UserSignUpDto
            {
                Name = "Test User",
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "password123"
            };

            _mockUserService.Setup(s => s.AddUser(userSignUpDto)).ReturnsAsync(string.Empty);

            var result = await _controller.SignUp(userSignUpDto);

            Assert.IsType<BadRequestObjectResult>(result);
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
            var result = await _controller.Login(new LoginRequest());

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
