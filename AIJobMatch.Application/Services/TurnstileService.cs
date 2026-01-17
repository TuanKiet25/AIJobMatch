using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.Services
{
    public class TurnstileService : ITurnstileService
    {
        private readonly HttpClient _httpClient;
        private readonly TurnstileSettings _settings;
        public TurnstileService(HttpClient httpClient, IOptions<TurnstileSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<bool> VerifyTokenAsync(string token)
        {
            if (!_settings.EnableCaptcha) return true;
            if (string.IsNullOrEmpty(token)) return false;

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("secret", _settings.SecretKey),
            new KeyValuePair<string, string>("response", token)
        });

            try
            {
                var response = await _httpClient.PostAsync(_settings.VerifyUrl, content);
                var result = await response.Content.ReadFromJsonAsync<TurnstileResponse>();
                return result?.Success ?? false;
            }
            catch
            {
                return false; // Fail-safe: Nếu server Cloudflare lỗi, có thể logic này cần cân nhắc
            }
        }
    }
}

