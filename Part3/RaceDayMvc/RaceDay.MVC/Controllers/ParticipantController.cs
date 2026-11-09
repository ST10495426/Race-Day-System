using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly ApiClient _api;

        public ParticipantController(ApiClient api)
        {
            _api = api;
        }

        // GET: /Participant/MyEvents
        public async Task<IActionResult> MyEvents()
        {
            // Check if user is logged in
            if (Request.Cookies[".AspNetCore.Session"] == null)
                return RedirectToAction("Login", "Account");

            // Fetch enrolments from API
            var response = await _api.GetAsync<List<object>>("api/users/me/enrolments");

            // For now, we pass the data as dynamic JSON to the view
            ViewBag.Enrolments = response;
            return View();
        }

        // GET: /Participant/MyResults
        public async Task<IActionResult> MyResults()
        {
            if (Request.Cookies[".AspNetCore.Session"] == null)
                return RedirectToAction("Login", "Account");

            var response = await _api.GetAsync<List<object>>("api/users/me/results");

            ViewBag.Results = response;
            return View();
        }
    }
}