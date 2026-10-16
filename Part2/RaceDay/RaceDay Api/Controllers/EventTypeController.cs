using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Api.Data;
using RaceDay_Api.Models;


namespace RaceDay_Api.Controllers
{
    // Public endpoint to fetch event types (Run, Walk, Cycle).
    // Used by Organisers when creating events and by Participants browsing.
    [Route("api/[controller]")]
    [ApiController]
    public class EventTypeController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventTypeController(RaceDayDbContext context)
        {
            _context = context;
        }

        // ============================================
        // GET: /api/eventtypes
        // Get all event types
        // ============================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var eventTypes = await _context.EventTypes.ToListAsync();
            return Ok(eventTypes);
        }
    }
}