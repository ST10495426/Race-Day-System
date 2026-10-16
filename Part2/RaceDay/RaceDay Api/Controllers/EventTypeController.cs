using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Api.Data;
using RaceDay_Api.Models;


namespace RaceDay_Api.Controllers
{
    /// <summary>
    /// Public lookup endpoint for event types (Run, Walk, Cycle).
    /// Used by Organisers when creating events and by Participants browsing.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EventTypeController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventTypeController(RaceDayDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all available event types.
        /// </summary>
        /// <response code="200">Returns the list of event types.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var eventTypes = await _context.EventTypes.ToListAsync();
            return Ok(eventTypes);
        }
    }
}