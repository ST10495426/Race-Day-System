using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class EnrolmentController : Controller
    {
        private readonly ApiClient _api;

        public EnrolmentController(ApiClient api)
        {
            _api = api;
        }

        [HttpPost]
        public async Task<IActionResult> Enrol(int EventID, int CategoryID)
        {
            var response = await _api.PostAsync($"api/events/{EventID}/enrol", new { CategoryID });

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("MyEvents", "Participant");
            }

            // If it failed (e.g., already enrolled)
            TempData["Error"] = "Could not enrol. You might already be enrolled.";
            return RedirectToAction("EventDetails", "Home", new { id = EventID });
        }
    }
}