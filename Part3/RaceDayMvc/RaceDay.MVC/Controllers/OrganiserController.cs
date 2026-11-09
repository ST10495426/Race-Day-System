using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly ApiClient _api;

        public OrganiserController(ApiClient api)
        {
            _api = api;
        }

        // GET: /Organiser/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            if (Request.Cookies[".AspNetCore.Session"] == null)
                return RedirectToAction("Login", "Account");

            // Fetch all events (we will filter them by organiser in the view for now)
            var events = await _api.GetAsync<List<EventListViewModel>>("api/events");

            return View(events ?? new List<EventListViewModel>());
        }

        // GET: /Organiser/CreateEvent
        public IActionResult CreateEvent()
        {
            if (Request.Cookies[".AspNetCore.Session"] == null)
                return RedirectToAction("Login", "Account");

            return View();

                    // ==========================================
                    // CATEGORIES
                    // ==========================================

            // GET: /Organiser/ManageCategories/5
        public async Task<IActionResult> ManageCategories(int id)
        {
            if (Request.Cookies[".AspNetCore.Session"] == null) return RedirectToAction("Login", "Account");

            var categories = await _api.GetAsync<List<CategoryViewModel>>($"api/events/{id}/categories");
            ViewBag.EventID = id;

            return View(categories ?? new List<CategoryViewModel>());
        }

        // POST: /Organiser/AddCategory
        [HttpPost]
        public async Task<IActionResult> AddCategory(int eventId, string name, string type, int? minValue, int? maxValue)
        {
            if (Request.Cookies[".AspNetCore.Session"] == null) return RedirectToAction("Login", "Account");

            var categoryData = new
            {
                Name = name,
                CategoryType = type,
                MinValue = minValue,
                MaxValue = maxValue
            };

            await _api.PostAsync($"api/events/{eventId}/categories", categoryData);

            return RedirectToAction("ManageCategories", new { id = eventId });
        }

        // ==========================================
        // ENROLMENTS
        // ==========================================

        // GET: /Organiser/ViewEnrolments/5
        public async Task<IActionResult> ViewEnrolments(int id)
        {
            if (Request.Cookies[".AspNetCore.Session"] == null) return RedirectToAction("Login", "Account");

            // We fetch the raw JSON array from the API
            var enrolments = await _api.GetAsync<List<object>>($"api/events/{id}/enrolments");
            ViewBag.EventID = id;

            return View(enrolments);
        }

        // ==========================================
        // RESULTS
        // ==========================================

        // GET: /Organiser/CaptureResult?enrolmentId=10
        public IActionResult CaptureResult(int enrolmentId)
        {
            if (Request.Cookies[".AspNetCore.Session"] == null) return RedirectToAction("Login", "Account");

            ViewBag.EnrolmentID = enrolmentId;
            return View();
        }

        // POST: /Organiser/CaptureResult
        [HttpPost]
        public async Task<IActionResult> CaptureResult(int enrolmentId, string finishTime, int? position, string status)
        {
            if (Request.Cookies[".AspNetCore.Session"] == null) return RedirectToAction("Login", "Account");

            var resultData = new
            {
                FinishTime = finishTime,
                Position = position,
                Status = status
            };

            var response = await _api.PostAsync($"api/enrolments/{enrolmentId}/results", resultData);

            // After saving, go back to the dashboard
            return RedirectToAction("Dashboard");
        }
        }

        // POST: /Organiser/CreateEvent
        [HttpPost]
        public async Task<IActionResult> CreateEvent(string Name, string Description, DateTime Date, string Location, string Distance, int EventTypeID)
        {
            if (Request.Cookies[".AspNetCore.Session"] == null)
                return RedirectToAction("Login", "Account");

            var eventData = new
            {
                Name = Name,
                Description = Description,
                Date = Date,
                Location = Location,
                Distance = Distance,
                EventTypeID = EventTypeID
            };

            var response = await _api.PostAsync("api/events", eventData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Could not create event. Please check your inputs.";
            return View();
        }
    }
}