using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ERP.Model;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service
{
    public interface ILlmService
    {
        Task<string> GenerateResponseAsync(string prompt);
    }

    public class LlmService : ILlmService
    {
        private readonly HttpClient _httpClient;
        public LlmService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            var requestBody = new
            {
                prompt = prompt,
                max_tokens = 2048,
            };
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));
            var response = await _httpClient.PostAsync("https://api.your-llm-service.com/generate", content); // placeholder
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApplicationDbContext.LlmResponse>(responseBody);
                if (result != null && !string.IsNullOrEmpty(result.Response))
                {
                    return result.Response;
                }
                else
                {
                    return "Error generating response";
                }
            }
            else
            {
                throw new HttpRequestException($"Error calling API: {response.StatusCode}");
            }
        }
    }
}
