using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApiClient _api;
        public ProfileController(ApiClient api) { _api = api; }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("ApiCookie") == null) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadPicture(IFormFile imageFile)
        {
            if (HttpContext.Session.GetString("ApiCookie") == null) return RedirectToAction("Login", "Account");

            if (imageFile != null && imageFile.Length > 0)
            {
                await _api.PostFileAsync("api/users/profile/picture", imageFile);
            }
            return RedirectToAction("Index");
        }
    }
}