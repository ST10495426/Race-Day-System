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
    // Handles participant enrolments.
    // Participant: enrol, withdraw, view own enrolments.
    // Organiser: view enrolments for own events.
    [Route("api")]
    [ApiController]
    public class EnrolmentController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentController(RaceDayDbContext context)
        {
            _context = context;
        }

        // ============================================
        // POST: /api/events/{eventId}/enrol
        // Participant only - enrol in an event
        // ============================================
        [HttpPost("events/{eventId}/enrol")]
        [RoleAuthorize("Participant")]
        public async Task<IActionResult> Enrol(int eventId, [FromBody] EnrolRequest request)
        {
            var participantId = SessionHelper.GetUserId(HttpContext.Session);
            if (participantId == null) return Unauthorized();

            // 1. Verify event exists
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound(new { message = "Event not found." });

            // 2. Verify category exists and belongs to this event
            var category = await _context.Categories.FindAsync(request.CategoryID);
            if (category == null || category.EventID != eventId)
                return NotFound(new { message = "Category not found for this event." });

            // 3. Check if already enrolled (unique constraint)
            var existing = await _context.Enrolments
                .FirstOrDefaultAsync(e => e.ParticipantID == participantId.Value && e.EventID == eventId);
            if (existing != null)
                return Conflict(new { message = "You are already enrolled in this event." });

            // 4. Create enrolment
            var enrolment = new Enrolment
            {
                ParticipantID = participantId.Value,
                EventID = eventId,
                CategoryID = request.CategoryID,
                EnrolmentDate = DateTime.Now,
                Status = "Confirmed"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = enrolment.EnrolmentID }, new
            {
                enrolment.EnrolmentID,
                enrolment.ParticipantID,
                enrolment.EventID,
                enrolment.CategoryID,
                enrolment.EnrolmentDate,
                enrolment.Status
            });
        }

        // ============================================
        // GET: /api/enrolments/{id}
        // Internal use for CreatedAtAction
        // ============================================
        [HttpGet("enrolments/{id}")]
        [RoleAuthorize("Organiser", "Participant")]
        public async Task<IActionResult> GetById(int id)
        {
            var enrolment = await _context.Enrolments.FindAsync(id);
            if (enrolment == null) return NotFound();
            return Ok(enrolment);
        }

        // ============================================
        // GET: /api/events/{eventId}/enrolments
        // Organiser only - view all enrolments for own event
        // ============================================
        [HttpGet("events/{eventId}/enrolments")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            var organiserId = SessionHelper.GetUserId(HttpContext.Session);
            if (organiserId == null) return Unauthorized();

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound(new { message = "Event not found." });

            // Only the owning organiser can view
            if (ev.OrganiserID != organiserId.Value)
                return Forbid();

            var enrolments = await _context.Enrolments
                .Where(en => en.EventID == eventId)
                .Include(en => en.Participant)
                .Include(en => en.Category)
                .Select(en => new
                {
                    en.EnrolmentID,
                    en.ParticipantID,
                    ParticipantName = en.Participant != null ? en.Participant.FullName : null,
                    CategoryName = en.Category != null ? en.Category.Name : null,
                    en.EnrolmentDate,
                    en.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // ============================================
        // GET: /api/users/me/enrolments
        // Participant only - view own enrolments
        // ============================================
        [HttpGet("users/me/enrolments")]
        [RoleAuthorize("Participant")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            var participantId = SessionHelper.GetUserId(HttpContext.Session);
            if (participantId == null) return Unauthorized();

            var enrolments = await _context.Enrolments
                .Where(en => en.ParticipantID == participantId.Value)
                .Include(en => en.Event)
                .Include(en => en.Category)
                .Select(en => new
                {
                    en.EnrolmentID,
                    en.EventID,
                    EventName = en.Event != null ? en.Event.Name : null,
                    CategoryName = en.Category != null ? en.Category.Name : null,
                    en.EnrolmentDate,
                    en.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // ============================================
        // PUT: /api/enrolments/{id}/withdraw
        // Participant only - withdraw from event
        // ============================================
        [HttpPut("enrolments/{id}/withdraw")]
        [RoleAuthorize("Participant")]
        public async Task<IActionResult> Withdraw(int id)
        {
            var participantId = SessionHelper.GetUserId(HttpContext.Session);
            if (participantId == null) return Unauthorized();

            var enrolment = await _context.Enrolments.FindAsync(id);
            if (enrolment == null) return NotFound(new { message = "Enrolment not found." });

            // Only the owning participant can withdraw
            if (enrolment.ParticipantID != participantId.Value)
                return Forbid();

            enrolment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Withdrawn successfully.", enrolmentID = enrolment.EnrolmentID, status = enrolment.Status });
        }
    }
}