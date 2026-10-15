using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Api.Data;
using RaceDay_Api.DTOs;
using RaceDay_Api.Filters;
using RaceDay_Api.Models;

namespace RaceDay_Api.Controllers
{
    // Handles event categories (age or distance groups).
    // Public: viewing categories.
    // Organiser only: create, update, delete.
    [Route("api")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoryController(RaceDayDbContext context)
        {
            _context = context;
        }

        // ============================================
        // GET: /api/events/{eventId}/categories
        // Public - all categories for an event
        // ============================================
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

        // ============================================
        // GET: /api/categories/{id}
        // Public - single category
        // ============================================
        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound(new { message = "Category not found." });
            return Ok(category);
        }

        // ============================================
        // POST: /api/events/{eventId}/categories
        // Organiser only - create category for own event
        // ============================================
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

        // ============================================
        // PUT: /api/categories/{id}
        // Organiser only
        // ============================================
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

        // ============================================
        // DELETE: /api/categories/{id}
        // Organiser only
        // ============================================
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