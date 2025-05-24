using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ERP.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ERP.Service
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<UserService> _logger;
        private readonly string _jwtSecret;

        public UserService(ApplicationDbContext dbContext, ILogger<UserService> logger, string jwtSecret)
        {
            _dbContext = dbContext;
            _logger = logger;
            _jwtSecret = jwtSecret;
        }

        public async Task<string> AddUser(ApplicationDbContext.UserDto userDto)
        {
            try
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                var user = new ApplicationDbContext.User
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    Password = userDto.Password,
                    Username = userDto.Username
                };

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("User {Username} added successfully with ID {User Id}.", user.Username, user.Id);
                return user.Id.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add user {Username}.", userDto.Username);
                return string.Empty;
            }
        }

        public async Task<string?> Login(string username, string Password)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.Password))
            {
                _logger.LogError("Failed to login user {Username}.", username);
                return null;
            }
            _logger.LogInformation("User {Username} logged in successfully.", username);
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(ApplicationDbContext.User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> UpdateUser (string userId, ApplicationDbContext.UserDto userDto)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogError("User {Username} not found.", userDto.Username);
                    return false;   
                }

                // Update user properties
                user.Name = userDto.Name;
                user.Email = userDto.Email;
                user.Password = userDto.Password;
                user.Username = userDto.Username;

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("User {Username} updated successfully.", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user {Username}.", userDto.Username);
                throw;
            }
        }

        public async Task<bool> DeleteUser (string userId)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {Username} not found.", userId);
                    return false;
                }
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("User {Username} deleted successfully.", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user {Username}.", userId);
                throw;
            }
        }
    }

    public interface IUserService
    {
        Task<string> AddUser (ApplicationDbContext.UserDto userDto);
        Task<bool> UpdateUser (string userId, ApplicationDbContext.UserDto userDto);
        Task<bool> DeleteUser (string userId);  
        Task<string?> Login(string username, string Password);
    }
}