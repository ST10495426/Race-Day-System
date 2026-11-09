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