using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ERP.Service.Tests
{
    public class UserControllerTest : IClassFixture<CustomWebAppFactory>
    {
        private readonly HttpClient _client;

        public UserControllerTest(CustomWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOk()
        {
            var response = await _client.GetAsync("api/User");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Add more tests for other UserController endpoints as needed
    }
}
