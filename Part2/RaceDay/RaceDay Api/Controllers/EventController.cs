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
    /// <summary>
    /// Handles event management.
    /// Public endpoints allow anyone to view events. Organiser endpoints allow CRUD operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventController(RaceDayDbContext context)
        {
            _context = context;
        }


        // GET: /api/events
        /// <summary>
        /// Gets all events with their categories included.
        /// </summary>
        /// <response code="200">Returns the list of events.</response>
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


        // GET: /api/events/{id}
        /// <summary>
        /// Gets a single event by its ID with categories.
        /// </summary>
        /// <param name="id">The event ID.</param>
        /// <response code="200">Returns the event details.</response>
        /// <response code="404">Event not found.</response>
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


        // POST: /api/events
        /// <summary>
        /// Creates a new event. Organiser only.
        /// </summary>
        /// <param name="request">Event details including name, description, date, location, distance, 
        /// and event type.</param>
        /// <response code="201">Event created successfully.</response>
        /// <response code="400">Validation failed or invalid EventTypeID.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">Not an Organiser.</response>
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

   
        // PUT: /api/events/{id}
        /// <summary>
        /// Updates an existing event. Organiser only, must own the event.
        /// </summary>
        /// <param name="id">The event ID.</param>
        /// <param name="request">Updated event details.</param>
        /// <response code="200">Event updated successfully.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">Not an Organiser or not the owner.</response>
        /// <response code="404">Event not found.</response>
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

        
        // DELETE: /api/events/{id}
        /// <summary>
        /// Deletes an event. Organiser only, must own the event.
        /// </summary>
        /// <param name="id">The event ID.</param>
        /// <response code="204">Event deleted successfully.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">Not an Organiser or not the owner.</response>
        /// <response code="404">Event not found.</response>
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