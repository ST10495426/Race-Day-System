using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Api.Data;
using RaceDay_Api.DTOs;
using RaceDay_Api.Filters;
using RaceDay_Api.Models;

namespace RaceDay_Api.Controllers
{
    /// <summary>
    /// Handles event categories (age or distance groups).
    /// Public viewing endpoints are open to all. CRUD operations are Organiser only.
    /// </summary>
    [Route("api")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoryController(RaceDayDbContext context)
        {
            _context = context;
        }

      
        // GET: /api/events/{eventId}/categories
        /// <summary>
        /// Gets all categories for a specific event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <response code="200">Returns the categories.</response>
        /// <response code="404">Event not found.</response>
        [HttpGet("events/{eventId}/categories")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound(new { message = "Event not found." });

            var categories = await _context.Categories
                .Where(c => c.EventID == eventId)
                .ToListAsync();

            return Ok(categories);
        }

        
        // GET: /api/categories/{id}
        /// <summary>
        /// Gets a single category by ID.
        /// </summary>
        /// <param name="id">The category ID.</param>
        /// <response code="200">Returns the category.</response>
        /// <response code="404">Category not found.</response>
        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound(new { message = "Category not found." });
            return Ok(category);
        }

       
        // POST: /api/events/{eventId}/categories
        /// <summary>
        /// Creates a new category for an event. Organiser only.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="request">Category details including name, type, and min/max values.</param>
        /// <response code="201">Category created successfully.</response>
        /// <response code="400">Invalid CategoryType.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">Not an Organiser.</response>
        /// <response code="404">Event not found.</response>
        [HttpPost("events/{eventId}/categories")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Create(int eventId, [FromBody] CreateCategoryRequest request)
        {
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound(new { message = "Event not found." });

            // Validate CategoryType
            if (request.CategoryType != "Age" && request.CategoryType != "Distance")
                return BadRequest(new { message = "CategoryType must be 'Age' or 'Distance'." });

            var category = new Category
            {
                EventID = eventId,
                Name = request.Name,
                CategoryType = request.CategoryType,
                MinValue = request.MinValue,
                MaxValue = request.MaxValue
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = category.CategoryID }, category);
        }

        
        // PUT: /api/categories/{id}
        /// <summary>
        /// Updates a category. Organiser only.
        /// </summary>
        /// <param name="id">The category ID.</param>
        /// <param name="request">Updated category details.</param>
        /// <response code="200">Category updated successfully.</response>
        /// <response code="400">Invalid CategoryType.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">Not an Organiser.</response>
        /// <response code="404">Category not found.</response>
        [HttpPut("categories/{id}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound(new { message = "Category not found." });

            if (request.CategoryType != "Age" && request.CategoryType != "Distance")
                return BadRequest(new { message = "CategoryType must be 'Age' or 'Distance'." });

            category.Name = request.Name;
            category.CategoryType = request.CategoryType;
            category.MinValue = request.MinValue;
            category.MaxValue = request.MaxValue;

            await _context.SaveChangesAsync();
            return Ok(category);
        }

       
        // DELETE: /api/categories/{id}
        /// <summary>
        /// Deletes a category. Organiser only.
        /// </summary>
        /// <param name="id">The category ID.</param>
        /// <response code="204">Category deleted successfully.</response>
        /// <response code="401">Not logged in.</response>
        /// <response code="403">Not an Organiser.</response>
        /// <response code="404">Category not found.</response>
        [HttpDelete("categories/{id}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound(new { message = "Category not found." });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}