using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RaceDay_Api.Data;
using RaceDay_Api.DTOs;
using RaceDay_Api.Models;
using RaceDay_Api.Services;

namespace RaceDay_Api.Controllers
{
    // This controller handles user registration, login, and logout.
    // Session-based auth: after login, we store UserID and Role in the session.
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly PasswordHasher _passwordHasher;

        public AuthController(RaceDayDbContext context, PasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // ============================================
        // POST: /api/auth/register
        // Register a new user (Organiser or Participant)
        // ============================================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // 1. Validate role
            if (request.Role != "Organiser" && request.Role != "Participant")
                return BadRequest(new { message = "Role must be 'Organiser' or 'Participant'." });

            // 2. Check if email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
                return Conflict(new { message = "Email already registered." });

            // 3. Create new user with hashed password
            var user = new User
            {
                Email = request.Email,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                FullName = request.FullName,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 4. Return response without password
            return CreatedAtAction(nameof(Register), new UserResponse
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
        // POST: /api/auth/login
        // Authenticate user and create session
        // ============================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            // 2. Verify password
            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password." });

            // 3. Store user in session
            SessionHelper.SetUser(HttpContext.Session, user.UserID, user.Role);

            // 4. Return user details
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
        // POST: /api/auth/logout
        // Clear session
        // ============================================
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            SessionHelper.Clear(HttpContext.Session);
            return Ok(new { message = "Logged out successfully." });
        }
    }
}