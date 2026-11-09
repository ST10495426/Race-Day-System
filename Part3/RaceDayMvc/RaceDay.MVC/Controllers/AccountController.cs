using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiClient _api;

        public AccountController(ApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Send login request to API
            var response = await _api.PostAsync("api/auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                // The ApiClient automatically grabbed the cookie and saved it in our Session.
                // Let's double check it actually saved.
                var hasCookie = HttpContext.Session.GetString("ApiCookie");

                if (string.IsNullOrEmpty(hasCookie))
                {
                    ModelState.AddModelError("", "Login succeeded, but session failed to save.");
                    return View(model);
                }
                // Get the user's role from the API response
                var content = await response.Content.ReadAsStringAsync();
                var userObj = System.Text.Json.JsonDocument.Parse(content).RootElement;
                var role = userObj.GetProperty("role").GetString();

                // Save it in the MVC session
                HttpContext.Session.SetString("UserRole", role ?? "Participant");

                // Success! Go to Home
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _api.PostAsync("api/auth/register", model);

            if (response.IsSuccessStatusCode)
            {
                // Automatically log them in after registering
                var loginModel = new LoginViewModel { Email = model.Email, Password = model.Password };
                await _api.PostAsync("api/auth/login", loginModel);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Registration failed. Email might already exist.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _api.PostAsync("api/auth/logout", new { });
            HttpContext.Session.Clear(); // Wipe the MVC session clean
            return RedirectToAction("Index", "Home");
        }
    }
}