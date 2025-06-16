using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ERP.Service.Tests
{
    public class LlmControllerTest : IClassFixture<CustomWebAppFactory>
    {
        private readonly HttpClient _client;

        public LlmControllerTest(CustomWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetLlmStatus_ShouldReturnOk()
        {
            var response = await _client.GetAsync("api/Llm/status");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Add more tests for other LlmController endpoints as needed
    }
}
