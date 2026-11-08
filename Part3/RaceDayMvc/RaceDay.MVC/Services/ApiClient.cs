using System.Text;
using System.Text.Json;

namespace RaceDay.MVC.Services
{
    // This class handles all communication with the Part 2 API.
    // It automatically forwards session cookies between the browser, MVC and the API.
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // Helps to get the cookie from the current browser request
        private void AttachSessionCookie()
        {
            var sessionCookie = _httpContextAccessor.HttpContext?.Request.Cookies[".AspNetCore.Session"];
            if (!string.IsNullOrEmpty(sessionCookie))
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.Session={sessionCookie}");
            }
        }

        // Helps to save the API's cookie into the browser's cookie collection
        private void SaveSessionCookie(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                {
                    if (cookie.StartsWith(".AspNetCore.Session"))
                    {
                        var cookieValue = cookie.Split(';')[0]; // Extract just the name=value
                        _httpContextAccessor.HttpContext?.Response.Headers.Append("Set-Cookie", cookieValue);
                    }
                }
            }
        }

        // GET Request
        public async Task<T?> GetAsync<T>(string url)
        {
            AttachSessionCookie();
            var response = await _httpClient.GetAsync(url);
            SaveSessionCookie(response);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return default;
        }

        // POST Request
        public async Task<HttpResponseMessage> PostAsync(string url, object data)
        {
            AttachSessionCookie();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            SaveSessionCookie(response);
            return response;
        }

        // PUT Request
        public async Task<HttpResponseMessage> PutAsync(string url, object? data = null)
        {
            AttachSessionCookie();
            HttpContent? content = null;
            if (data != null)
            {
                var json = JsonSerializer.Serialize(data);
                content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.PutAsync(url, content);
            SaveSessionCookie(response);
            return response;
        }
    }
}
