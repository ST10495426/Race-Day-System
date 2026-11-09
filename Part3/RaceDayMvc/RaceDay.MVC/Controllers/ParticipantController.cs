using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
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

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("ApiCookie") != null;
        }

        // GET: /Participant/MyEvents
        public async Task<IActionResult> MyEvents()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            // Fetch from the API
            var enrolments = await _api.GetAsync<List<object>>("api/users/me/enrolments");

            // Parse the JSON to a list of dictionaries for the view
            var parsedEnrolments = new List<Dictionary<string, object>>();
            if (enrolments != null)
            {
                foreach (var item in enrolments)
                {
                    var json = JsonSerializer.Serialize(item);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    if (dict != null) parsedEnrolments.Add(dict);
                }
            }

            return View(parsedEnrolments);
        }

        // GET: /Participant/MyResults
        public async Task<IActionResult> MyResults()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var results = await _api.GetAsync<List<object>>("api/users/me/results");

            var parsedResults = new List<Dictionary<string, object>>();
            if (results != null)
            {
                foreach (var item in results)
                {
                    var json = JsonSerializer.Serialize(item);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    if (dict != null) parsedResults.Add(dict);
                }
            }

            return View(parsedResults);
        }
    }
}