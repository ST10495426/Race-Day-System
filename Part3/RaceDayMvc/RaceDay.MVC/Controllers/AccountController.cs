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

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
           return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task <IActionResult> Login( LoginViewModel model)
        {
            if (!ModelState.IsValid ) return View( model);

            // Send data to API
            var response =await _api.PostAsync ("api/auth/login", model);

            if  (response.IsSuccessStatusCode)
            {
                // API returns a cookie. Our ApiClient handled saving it to the browser.
                return RedirectToAction ("Index", "Home");
            }

            // If failed (401 Unauthorized)
            ModelState.AddModelError("","Invalid email or password." );
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
             return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _api.PostAsync("api/auth/register", model);

            if (response.IsSuccessStatusCode)
            {
                // Automatically log them in after registering
                var loginModel = new LoginViewModel { Email = model.Email, Password = model.Password };
                await _api.PostAsync ("api/auth/login", loginModel);

                return RedirectToAction ("Index", "Home");
            }

            ModelState.AddModelError("","Registration failed. Email might already exist." );
            return  View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Tell API to clear session
            await _api.PostAsync ( "api/auth/logout",new { });

            // Clear browser cookies 
            Response.Cookies.Delete(".AspNetCore.Session" );

            return RedirectToAction("Login");
        }
    }
}