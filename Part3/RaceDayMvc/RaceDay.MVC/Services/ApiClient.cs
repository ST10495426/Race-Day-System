using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

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
        private void SaveCookie(HttpResponseMessage response)
        {
            // Check if the API sent a Set-Cookie header
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                {
                    // Ignore the expiration cookie, we only want the actual value
                    if (cookie.Contains(".AspNetCore.Session=") && !cookie.Contains(".AspNetCore.Session=;"))
                    {
                        // Extract the value between '=' and ';'
                        var startIndex = cookie.IndexOf(".AspNetCore.Session=") + ".AspNetCore.Session=".Length;
                        var endIndex = cookie.IndexOf(';', startIndex);

                        if (endIndex == -1) endIndex = cookie.Length;

                        var cookieValue = cookie.Substring(startIndex, endIndex - startIndex);

                        // Save to MVC Session
                        _httpContextAccessor.HttpContext?.Session.SetString("ApiCookie", cookieValue);
                        break;
                    }
                }
            }
        }

        private void AttachCookie()
        {
            var cookie = _httpContextAccessor.HttpContext?.Session.GetString("ApiCookie");
            if (!string.IsNullOrEmpty(cookie))
            {
                _http.DefaultRequestHeaders.Clear();
                // Send the cookie exactly as the API expects it
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
            else
            {
                // Log the error for debugging
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error on GET {url}: {response.StatusCode} - {error}");
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
        // method for uploading files (images) to the API
        public async Task<HttpResponseMessage> PostFileAsync(string url, IFormFile file)
        {
            AttachCookie(); // Make sure we send the session cookie!

            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();

            // The API expects a parameter named "file"
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            var response = await _http.PostAsync(url, content);
            SaveCookie(response);
            return response;
        }
    }
}