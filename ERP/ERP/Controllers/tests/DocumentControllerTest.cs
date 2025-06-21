using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ERP.Service.Tests
{
    public class DocumentControllerTest : IClassFixture<CustomWebAppFactory>
    {
        private readonly HttpClient _client;

        public DocumentControllerTest(CustomWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetDocuments_ShouldReturnOk()
        {
            var response = await _client.GetAsync("api/Document");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Add more tests for other DocumentController endpoints as needed
    }
}
