using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Diagnostics;

namespace RaceDay.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiClient _api;

        public HomeController(ApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch events from API
            var events = await _api.GetAsync<List<EventListViewModel>>("api/Event");

            if (events == null)
            {
                events = new List<EventListViewModel>();
            }

            return View(events);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> EventDetails(int id)
        {
            var ev = await _api.GetAsync<EventDetailsViewModel>($"api/Event/{id}");
            if (ev == null) return NotFound();

            return View(ev);
        }
    }
}
