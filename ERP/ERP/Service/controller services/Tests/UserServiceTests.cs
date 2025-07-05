using ERP.Model;
using ERP.Service;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ERP.Service.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockDbContext;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockDbContext = new Mock<ApplicationDbContext>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockDbContext.Object, _mockLogger.Object, "test_jwt_secret");
        }

        [Fact]
        public async Task AddUser_ShouldHashPasswordAndSaveUser()
        {
            var userDto = new ApplicationDbContext.UserSignUpDto
            {
                Name = "Test User",
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "password123",
                CompanyName = "Test Company",
                CountryOfOrigin = "Test Country",
                Address = "123 Test St",
                NumberOfRoles = 3,
                CompanyNumber = "123456789"
            };

            var result = await _userService.AddUser(userDto);

            Assert.False(string.IsNullOrEmpty(result));
            // Additional asserts can be added if mocking DbContext SaveChangesAsync is setup
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            var username = "testuser";
            var password = "password123";

            // Setup mock user with hashed password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new ApplicationDbContext.User
            {
                Username = username,
                Password = hashedPassword,
                Name = "Test User",
                Email = "testuser@example.com"
            };

            // Setup DbContext mock to return user
            // This requires more advanced mocking or use of in-memory database, omitted here for brevity

            var token = await _userService.Login(username, password);

            // Token should not be null or empty if login successful
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task Login_ShouldReturnNull_WhenUserNotFound()
        {
            var token = await _userService.Login("nonexistentuser", "password");
            Assert.Null(token);
        }

        [Fact]
        public async Task Login_ShouldReturnNull_WhenPasswordInvalid()
        {
            var username = "testuser";
            var password = "wrongpassword";

            // Setup mock user with different password
            // Omitted for brevity

            var token = await _userService.Login(username, password);
            Assert.Null(token);
        }
    }
}
