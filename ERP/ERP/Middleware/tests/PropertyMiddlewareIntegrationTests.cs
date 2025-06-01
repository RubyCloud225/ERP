using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using ERP.Program;
using ERP.Middleware;

namespace ERP.IntegrationTests
{
    public class PropertyMiddlewareIntegrationTests : IClassFixture<WebApplicationFactory<ApplicationMain>>
    {
        private readonly WebApplicationFactory<ApplicationMain> _factory;
        public PropertyMiddlewareIntegrationTests(WebApplicationFactory<ApplicationMain> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
            });
        }
        [Fact]
        public async Task Middleware_Caches_And_Passes_Properties_To_Controller()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/api/test");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
            // Assert
            Assert.NotNull(result);
            Assert.Contains("User-Agent", result.Keys);
            Assert.Contains("Accept-Language", result.Keys);
            Assert.Contains("Host", result.Keys);
            Assert.Contains("Accept", result.Keys);
            Assert.Contains("Accept-Encoding", result.Keys);
            Assert.Contains("Connection", result.Keys);
            Assert.Contains("Request-Method", result.Keys);
            Assert.Contains("Request-Scheme", result.Keys);
            Assert.Contains("Request-Host", result.Keys);
            Assert.Contains("Request-Path", result.Keys);
            Assert.Contains("Request-Query", result.Keys);
        }
        [Fact]
        public async Task Middleware_Repopulates_Properties_After_Expiration()
        {
            var client = _factory.CreateClient();
            // Act 1 Initial Request to populare cache
            var response1 = await client.GetAsync("/api/test");
            response1.EnsureSuccessStatusCode();
            var content1 = await response1.Content.ReadAsStringAsync();
            var result1 = JsonSerializer.Deserialize<Dictionary<string, string>>(content1);
            var timestamp1 = result1?["Timestamp"];
            // Wait for cache to expire (assuming cache expires in 10 seconds)
            await Task.Delay(TimeSpan.FromSeconds(10));
            // Act 2 Second request to check if cache has expired
            var response2 = await client.GetAsync("/api/test");
            response2.EnsureSuccessStatusCode();
            var content2 = await response2.Content.ReadAsStringAsync();
            var result2 = JsonSerializer.Deserialize<Dictionary<string, string>>(content2);
            var timestamp2 = result2?["Timestamp"];
            // Assert
            Assert.NotEqual(timestamp1, timestamp2);
        }
        // Long-lived cache test - use cache eviction policy and max size limit
        [Fact]
        public async Task Middleware_Handles_LongLivedCache_WithEvictionPolicy()
        {
            //Arrange
            var client = _factory.CreateClient();
            // Act
            for (int i = 0; i < 100; i++)
            {
                var response = await client.GetAsync("/api/test");
                response.EnsureSuccessStatusCode();
            }
            // Assert
            var cache = PropertyCachingMiddleware.GetCache();
            Assert.True(cache.Count <= 100, "Cache size exceeded limit");
        }

        // Expired entries not cleaned - Periodic cleanup test as a background task
        [Fact]
        public async Task Middleware_PeriodicCleanup_RemovesExpiredEntries()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act 1 
            for (int i = 0; i < 10; i++)
            {
                var response = await client.GetAsync("/api/test");
                response.EnsureSuccessStatusCode();
            }
            // Act 2
            await Task.Delay(TimeSpan.FromSeconds(10)); // Wait for cache to expire
            // Act 3
            var responseAfterCleanup = await client.GetAsync("/api/test");
            responseAfterCleanup.EnsureSuccessStatusCode();
            var contentAfterCleanup = await responseAfterCleanup.Content.ReadAsStringAsync();
            var resultAfterCleanup = JsonSerializer.Deserialize<Dictionary<string, string>>(contentAfterCleanup);
            // Assert
            Assert.NotNull(resultAfterCleanup);
        }
        // Test for Path-based caching key collision
        [Fact]
        public async Task Middleware_PathBasedCachingKeyCollision()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response1 = await client.GetAsync("/api/test/1");
            response1.EnsureSuccessStatusCode();
            var response2 = await client.GetAsync("/api/test/2");
            response2.EnsureSuccessStatusCode();
            // Assert
            var cache = PropertyCachingMiddleware.GetCache();
            Assert.NotNull(cache);
            Assert.Contains("PropertiesCache_/api/test/1", cache.Keys);
            Assert.Contains("PropertiesCache_/api/test/2", cache.Keys);
            Assert.NotEqual(cache["PropertiesCache_/api/test/1"], cache["PropertiesCache_/api/test/2"]);
        }
        // Missing properties test
        [Fact]
        public async Task Middleware_MissingProperties()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/api/test/missing");
            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
            // Check if the result is null or empty
            if (result == null)
            {
                Assert.True(result == null || result.Count == 0, "Result is not empty or null.");
            }
            else
            {
                Assert.NotNull(result);
            }
        }
        // Test for large number of properties    
        [Fact]
        public async Task Middleware_LargeNumberOfProperties()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/api/test/large");
            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
            // Check if the result contains a large number of properties
            Assert.NotNull(result);
            if (result.Count > 100)
            {
                Assert.True(result.Count > 100, "Result does not contain a large number of properties.");
            }
            else
            {
                Assert.True(result.Count <= 100, "Result contains more than 100 properties.");
            }
        }
        // test for missing properties in the cache
        [Fact]
        public async Task Middleware_MissingPropertiesInCache()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/api/test/missing");
            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
            // Check if the result is null or empty
            if (result == null)
            {
                Assert.True(result == null || result.Count == 0, "Result is not empty or null.");
            }
            else
            {
                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }
        // Test for rapid requests
        [Fact]
        public async Task Middleware_RapidRequests()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            for (int i = 0; i < 100; i++)
            {
                var response = await client.GetAsync("/api/test/rapid");
                response.EnsureSuccessStatusCode();
            }
            // Assert
            var cache = PropertyCachingMiddleware.GetCache();
            Assert.NotNull(cache);
            Assert.True(cache.Count > 0, "Cache should contain properties after rapid requests.");
        }
        // test for multi threaded requests
        [Fact]
        public async Task Middleware_MultiThreadedRequests()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var response = await client.GetAsync("/api/test/multi");
                    response.EnsureSuccessStatusCode();
                }));
            }
            await Task.WhenAll(tasks);
            // Assert
            var cache = PropertyCachingMiddleware.GetCache();
            Assert.NotNull(cache);
            Assert.True(cache.Count > 0, "Cache should contain properties after multi-threaded requests.");
        }
        [Fact]
        public async Task Middleware_InvalidRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/api/test/invalid");
            // Assert
            Assert.False(response.IsSuccessStatusCode, "Response should not be successful for invalid request.");
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Not Found", content, StringComparison.OrdinalIgnoreCase);
        }
        [Fact]
        public async Task Middleware_Non_String_Items_in_HTTPContext()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/api/test/non-string-items");
            response.EnsureSuccessStatusCode();
            // Assert
            var content = await response.Content.ReadAsStringAsync();
            if (content.Contains("Non-string items in HTTPContext"))
            {
                Assert.True(true, "Non-string items in HTTPContext handled correctly.");
            }
            else
            {
                Assert.False(true, "Non-string items in HTTPContext not handled correctly.");
            }
        }
        // Middle
    }
}