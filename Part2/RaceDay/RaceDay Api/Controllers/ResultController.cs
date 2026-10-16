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
    // Handles race results.
    // Organiser: capture and update results, view all results for own events.
    // Participant: view own results.
    [Route("api")]
    [ApiController]
    public class ResultController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultController(RaceDayDbContext context)
        {
            _context = context;
        }

        
        // POST: /api/enrolments/{enrolmentId}/results
        /// <summary>
        /// Organiser only can capture result for an enrolment
        /// </summary>
        /// <response code="201">Enrolment created successfully.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">Not a Participant.</response>
        /// <response code="404">Event or category not found.</response>
        /// <response code="409">Already enrolled in this event.</response>
        [HttpPost("enrolments/{enrolmentId}/results")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Create(int enrolmentId, [FromBody] CreateResultRequest request)
        {
            // 1. Verify enrolment exists
            var enrolment = await _context.Enrolments.FindAsync(enrolmentId);
            if (enrolment == null) return NotFound(new { message = "Enrolment not found." });

            // 2. Check if result already exists (one-to-one)
            var existing = await _context.Results
                .FirstOrDefaultAsync(r => r.EnrolmentID == enrolmentId);
            if (existing != null)
                return Conflict(new { message = "A result already exists for this enrolment." });

            // 3. Validate status
            var validStatuses = new[] { "Completed", "DNF", "DQ", "DNS" };
            if (!validStatuses.Contains(request.Status))
                return BadRequest(new { message = "Status must be Completed, DNF, DQ, or DNS." });

            // 4. Create result
            var result = new Result
            {
                EnrolmentID = enrolmentId,
                FinishTime = request.FinishTime,
                Position = request.Position,
                Status = request.Status
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = result.ResultID }, new
            {
                result.ResultID,
                result.EnrolmentID,
                FinishTime = result.FinishTime?.ToString(),
                result.Position,
                result.Status
            });
        }

        
        // GET: /api/results/{id}
        // Internal use
        [HttpGet("results/{id}")]
        [RoleAuthorize("Organiser", "Participant")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        
        // PUT: /api/results/{id}
        // Organiser only - update result        
        [HttpPut("results/{id}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateResultRequest request)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null) return NotFound(new { message = "Result not found." });

            var validStatuses = new[] { "Completed", "DNF", "DQ", "DNS" };
            if (!validStatuses.Contains(request.Status))
                return BadRequest(new { message = "Status must be Completed, DNF, DQ, or DNS." });

            result.FinishTime = request.FinishTime;
            result.Position = request.Position;
            result.Status = request.Status;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                result.ResultID,
                result.EnrolmentID,
                FinishTime = result.FinishTime?.ToString(),
                result.Position,
                result.Status
            });
        }

        
        // GET: /api/users/me/results
        // Participant only - view own results
        [HttpGet("users/me/results")]
        [RoleAuthorize("Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var participantId = SessionHelper.GetUserId(HttpContext.Session);
            if (participantId == null) return Unauthorized();

            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(en => en!.Event)
                .Include(r => r.Enrolment)
                    .ThenInclude(en => en!.Category)
                .Where(r => r.Enrolment!.ParticipantID == participantId.Value)
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrolmentID,
                    EventID = r.Enrolment!.EventID,
                    EventName = r.Enrolment.Event != null ? r.Enrolment.Event.Name : null,
                    CategoryName = r.Enrolment.Category != null ? r.Enrolment.Category.Name : null,
                    FinishTime = r.FinishTime != null ? r.FinishTime.ToString() : null,
                    r.Position,
                    r.Status,
                    // Count total finishers for this event
                    TotalFinishers = _context.Results
                        .Count(res => res.Enrolment!.EventID == r.Enrolment.EventID
                            && res.Status == "Completed")
                })
                .ToListAsync();

            return Ok(results);
        }

       
        // GET: /api/events/{eventId}/results
        // Organiser only - view all results for own event
        
        [HttpGet("events/{eventId}/results")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            var organiserId = SessionHelper.GetUserId(HttpContext.Session);
            if (organiserId == null) return Unauthorized();

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound(new { message = "Event not found." });

            if (ev.OrganiserID != organiserId.Value)
                return Forbid();

            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(en => en!.Participant)
                .Where(r => r.Enrolment!.EventID == eventId)
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrolmentID,
                    ParticipantName = r.Enrolment!.Participant != null ? r.Enrolment.Participant.FullName : null,
                    FinishTime = r.FinishTime != null ? r.FinishTime.ToString() : null,
                    r.Position,
                    r.Status
                })
                .ToListAsync();

            return Ok(results);
        }
    }
}