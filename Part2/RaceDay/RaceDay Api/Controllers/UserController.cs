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
    /// <summary>
    /// Handles user profile operations for both roles.
    /// Authenticated users can view and update their own profile.
    /// </summary>
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

        /// <summary>
        /// Gets the currently logged-in user's profile.
        /// </summary>
        /// <response code="200">Returns the user's profile details.</response>
        /// <response code="401">Not logged in.</response>
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

        /// <summary>
        /// Updates the currently logged-in user's profile.
        /// </summary>
        /// <param name="request">Fields to update - full name and phone number.</param>
        /// <response code="200">Profile updated successfully.</response>
        /// <response code="400">Validation failed.</response>
        /// <response code="401">Not logged in.</response>
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