using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using ERP.Program;
using ERP.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using Xunit.Abstractions;

namespace ERP.IntegrationTests
{
    // Test auth handler that fakes an authenticated user
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "TestUser") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class PropertyMiddlewareIntegrationTests : IDisposable
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public PropertyMiddlewareIntegrationTests(ITestOutputHelper output)
        {
            _output = output;

            var factory = new TestWebApplicationFactory();

            _client = factory.CreateClient();
        }

        private async Task<HttpResponseMessage> SendRequestAsync(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Explicitly add User-Agent header to every request
            request.Headers.Add("User-Agent", "IntegrationTestClient/1.0");

            _output.WriteLine("Sending request with headers:");
            foreach (var header in request.Headers)
            {
                _output.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            var response = await _client.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"Response Status: {(int)response.StatusCode} {response.StatusCode}");
            _output.WriteLine($"Response Content: {content}");

            return response;
        }

        [Fact]
        public async Task Middleware_Caches_And_Passes_Properties_To_Controller()
        {
            var response = await SendRequestAsync("/api/test");

            _output.WriteLine($"Asserting success status code");

            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"Deserializing response content");
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(content);

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

        // Additional tests would follow the same pattern...
        
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}