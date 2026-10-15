using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Api.Data;
using RaceDay_Api.DTOs;
using RaceDay_Api.Filters;
using RaceDay_Api.Services;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaceDay_Api.Controllers
{
    // Handles user profile operations.
    // Both roles can view and update their OWN profile.
    [Route("api/[controller]")]
    [ApiController]
    [RoleAuthorize("Organiser", "Participant")]
    public class UserController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public UserController(RaceDayDbContext context)
        {
            _context = context;
        }

        // ============================================
        // GET: /api/users/profile
        // Get the currently logged-in user's profile
        // ============================================
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = SessionHelper.GetUserId(HttpContext.Session);
            if (userId == null) return Unauthorized();

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null) return NotFound();

            return Ok(new UserResponse
            {
                UserID = user.UserID,
                Email = user.Email,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                ProfilePictureUrl = user.ProfilePictureUrl,
                CreatedDate = user.CreatedDate
            });
        }

        // ============================================
        // PUT: /api/users/profile
        // Update the currently logged-in user's profile
        // ============================================
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = SessionHelper.GetUserId(HttpContext.Session);
            if (userId == null) return Unauthorized();

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null) return NotFound();

            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;

            await _context.SaveChangesAsync();

            return Ok(new UserResponse
            {
                UserID = user.UserID,
                Email = user.Email,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                ProfilePictureUrl = user.ProfilePictureUrl,
                CreatedDate = user.CreatedDate
            });
        }
    }
}