using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.Services
{
    public class BotssaApiService : IBotssaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BotssaApiService> _logger;

        public BotssaApiService(HttpClient httpClient, ILogger<BotssaApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetAuthTokenAsync()
        {
            var authRequest = new
            {
                userNameOrEmailAddress = "admin",
                password = "123qwe"
            };
            var content = new StringContent(JsonSerializer.Serialize(authRequest), Encoding.UTF8, "application/json-patch+json");
            var response = await _httpClient.PostAsync("https://localhost:44362/api/TokenAuth/Authenticate", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var authResult = JsonSerializer.Deserialize<ApiResult<AuthResult>>(json);
            return authResult?.Result.AccessToken;
        }

        public async Task<PromoCodeResult> ValidatePromoCodeAsync(string promoCode)
        {
            var token = await GetAuthTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:44362/api/dynamic/Boxfusion.Botsa/PromotionCode/Crud/Get?id={promoCode}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var promoCodeResult = JsonSerializer.Deserialize<ApiResult<PromoCodeResult>>(json);
                return promoCodeResult?.Result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error validating promo code: {ex.Message}");
                return null;
            }
        }
    }

    public class AuthResult
    {
        public string AccessToken { get; set; }
    }
}
