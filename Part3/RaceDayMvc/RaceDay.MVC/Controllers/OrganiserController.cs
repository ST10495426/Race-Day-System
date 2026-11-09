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

        // Helpe method to check if user is logged in
        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("ApiCookie") != null;
        }

        // GET: /Organiser/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var events = await _api.GetAsync<List<EventListViewModel>>("api/events");
            return View(events ?? new List<EventListViewModel>());
        }

        // GET: /Organiser/CreateEvent
        public IActionResult CreateEvent()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: /Organiser/CreateEvent
        [HttpPost]
        public async Task<IActionResult> CreateEvent(string Name, string Description, DateTime Date, string Location, string Distance, int EventTypeID)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            try
            {
                var eventData = new
                {
                    Name = Name,
                    Description = Description,
                    Date = Date.ToString("yyyy-MM-ddTHH:mm:ss"),
                    Location = Location,
                    Distance = Distance,
                    EventTypeID = EventTypeID
                };

                var response = await _api.PostAsync("api/events", eventData);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Dashboard");
                }

                // Read the error
                var errorDetails = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(errorDetails))
                {
                    errorDetails = "The API returned an empty error. Check the API Console window in Visual Studio for the crash log.";
                }

                ViewBag.Error = $"Status: {response.StatusCode} | Details: {errorDetails}";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception: " + ex.Message;
            }

            return View();
        }

        // ==========================================
        // CATEGORIES
        // ==========================================

        // GET: /Organiser/ManageCategories/5
        public async Task<IActionResult> ManageCategories(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var categories = await _api.GetAsync<List<CategoryViewModel>>($"api/events/{id}/categories");
            ViewBag.EventID = id;

            return View(categories ?? new List<CategoryViewModel>());
        }

        // POST: /Organiser/AddCategory
        [HttpPost]
        public async Task<IActionResult> AddCategory(int eventId, string name, string type, string minValue, string maxValue)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            // Convert empty strings to null for the API
            int? min = string.IsNullOrEmpty(minValue) ? null : int.Parse(minValue);
            int? max = string.IsNullOrEmpty(maxValue) ? null : int.Parse(maxValue);

            var categoryData = new
            {
                Name = name,
                CategoryType = type,
                MinValue = min,
                MaxValue = max
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
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

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
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            ViewBag.EnrolmentID = enrolmentId;
            return View();
        }

        // POST: /Organiser/CaptureResult
        [HttpPost]
        public async Task<IActionResult> CaptureResult(int enrolmentId, string finishTime, int? position, string status)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            // Format finishTime if provided (API expects HH:MM:SS)
            string? formattedTime = null;
            if (!string.IsNullOrEmpty(finishTime))
            {
                // If user typed "02:15:30", TimeSpan can parse it
                if (TimeSpan.TryParse(finishTime, out var ts))
                {
                    formattedTime = ts.ToString(@"hh\:mm\:ss");
                }
            }

            var resultData = new
            {
                FinishTime = formattedTime,
                Position = position,
                Status = status
            };

            await _api.PostAsync($"api/enrolments/{enrolmentId}/results", resultData);

            return RedirectToAction("Dashboard");
        }
    }
}