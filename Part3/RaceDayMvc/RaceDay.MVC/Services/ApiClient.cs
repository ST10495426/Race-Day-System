using System.Text;
using System.Text.Json;

namespace RaceDay.MVC.Services
{
    // This class handles all communication with the Part 2 API.
    // It automatically forwards session cookies between the browser, MVC and the API.
    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiClient(HttpClient http, IHttpContextAccessor httpContextAccessor)
        {
            _http = http;
            _httpContextAccessor = httpContextAccessor;
        }

        // Extract the cookie from the API response
        private void SaveCookie(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                {
                    if (cookie.Contains(".AspNetCore.Session"))
                    {
                        // Extract just the token value (between '=' and ';')
                        var parts = cookie.Split(';')[0].Split('=');
                        if (parts.Length > 1)
                        {
                            var cookieValue = parts[1];
                            _httpContextAccessor.HttpContext?.Session.SetString("ApiCookie", cookieValue);
                        }
                    }
                }
            }
        }

        // Attach the cookie to the request
        private void AttachCookie()
        {
            var cookie = _httpContextAccessor.HttpContext?.Session.GetString("ApiCookie");
            if (!string.IsNullOrEmpty(cookie))
            {
                _http.DefaultRequestHeaders.Clear();
                _http.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.Session={cookie}");
            }
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            AttachCookie();
            var response = await _http.GetAsync(url);
            SaveCookie(response);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return default;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, object data)
        {
            AttachCookie();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(url, content);
            SaveCookie(response);
            return response;
        }
    }
}