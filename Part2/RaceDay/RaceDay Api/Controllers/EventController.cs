using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Api.Data;
using RaceDay_Api.DTOs;
using RaceDay_Api.Filters;
using RaceDay_Api.Models;
using RaceDay_Api.Services;

namespace RaceDay_Api.Controllers
{
    // Handles event management.
    // Public: viewing events.
    // Organiser only: create, update, delete.
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventController(RaceDayDbContext context)
        {
            _context = context;
        }

        // ============================================
        // GET: /api/events
        // Public - get all events with categories
        // ============================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.Categories)
                .Select(e => new
                {
                    e.EventID,
                    e.Name,
                    e.Description,
                    e.Date,
                    e.Location,
                    e.Distance,
                    e.BannerUrl,
                    e.OrganiserID,
                    e.EventTypeID,
                    EventTypeName = e.EventType != null ? e.EventType.Name : null,
                    Categories = e.Categories.Select(c => new
                    {
                        c.CategoryID,
                        c.Name,
                        c.CategoryType,
                        c.MinValue,
                        c.MaxValue
                    })
                })
                .ToListAsync();

            return Ok(events);
        }

        // ============================================
        // GET: /api/events/{id}
        // Public - get single event with categories
        // ============================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ev = await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (ev == null) return NotFound(new { message = "Event not found." });

            return Ok(new
            {
                ev.EventID,
                ev.Name,
                ev.Description,
                ev.Date,
                ev.Location,
                ev.Distance,
                ev.BannerUrl,
                ev.OrganiserID,
                ev.EventTypeID,
                EventTypeName = ev.EventType?.Name,
                Categories = ev.Categories.Select(c => new
                {
                    c.CategoryID,
                    c.Name,
                    c.CategoryType,
                    c.MinValue,
                    c.MaxValue
                })
            });
        }

        // ============================================
        // POST: /api/events
        // Organiser only - create new event
        // ============================================
        [HttpPost]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
        {
            var organiserId = SessionHelper.GetUserId(HttpContext.Session);
            if (organiserId == null) return Unauthorized();

            // Verify EventType exists
            var eventType = await _context.EventTypes.FindAsync(request.EventTypeID);
            if (eventType == null)
                return BadRequest(new { message = "Invalid EventTypeID." });

            var ev = new Event
            {
                OrganiserID = organiserId.Value,
                EventTypeID = request.EventTypeID,
                Name = request.Name,
                Description = request.Description,
                Date = request.Date,
                Location = request.Location,
                Distance = request.Distance,
                CreatedDate = DateTime.Now
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = ev.EventID }, new
            {
                ev.EventID,
                ev.Name,
                ev.Description,
                ev.Date,
                ev.Location,
                ev.Distance,
                ev.EventTypeID,
                ev.OrganiserID
            });
        }

        // ============================================
        // PUT: /api/events/{id}
        // Organiser only - update own event
        // ============================================
        [HttpPut("{id}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEventRequest request)
        {
            var organiserId = SessionHelper.GetUserId(HttpContext.Session);
            if (organiserId == null) return Unauthorized();

            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound(new { message = "Event not found." });

            // Only the organiser who created it can update
            if (ev.OrganiserID != organiserId.Value)
                return Forbid();

            ev.Name = request.Name;
            ev.Description = request.Description;
            ev.Date = request.Date;
            ev.Location = request.Location;
            ev.Distance = request.Distance;
            ev.EventTypeID = request.EventTypeID;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Event updated successfully.", eventId = ev.EventID });
        }

        // ============================================
        // DELETE: /api/events/{id}
        // Organiser only - delete own event
        // ============================================
        [HttpDelete("{id}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var organiserId = SessionHelper.GetUserId(HttpContext.Session);
            if (organiserId == null) return Unauthorized();

            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound(new { message = "Event not found." });

            if (ev.OrganiserID != organiserId.Value)
                return Forbid();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}