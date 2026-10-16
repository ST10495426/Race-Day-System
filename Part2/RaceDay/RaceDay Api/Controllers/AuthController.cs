using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RaceDay_Api.Data;
using RaceDay_Api.DTOs;
using RaceDay_Api.Models;
using RaceDay_Api.Services;

namespace RaceDay_Api.Controllers
{
    /// <summary>
    /// Handles user registration, login, and logout.
    /// Uses session-based authentication - after login, the user's ID and role
    /// are stored server-side and identified by a session cookie.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly PasswordHasher _passwordHasher;

        public AuthController(RaceDayDbContext context, PasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <remarks>
        /// The role must be either "Organiser" or "Participant".
        /// Passwords are hashed with BCrypt before storage.
        /// </remarks>
        /// <param name="request">Registration details including email, password, full name, date of birth, phone number, and role.</param>
        /// <response code="201">User registered successfully. Returns the new user (without password).</response>
        /// <response code="400">Validation failed - invalid role or missing fields.</response>
        /// <response code="409">Email is already registered.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request.Role != "Organiser" && request.Role != "Participant")
                return BadRequest(new { message = "Role must be 'Organiser' or 'Participant'." });

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
                return Conflict(new { message = "Email already registered." });

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

        /// <summary>
        /// Logs in a user and creates a session.
        /// </summary>
        /// <remarks>
        /// If successful, a session cookie is set that must be included in all subsequent requests.
        /// The session stores the user's ID and role.
        /// </remarks>
        /// <param name="request">Login credentials - email and password.</param>
        /// <response code="200">Login successful. Returns the user details.</response>
        /// <response code="401">Invalid email or password.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password." });

            SessionHelper.SetUser(HttpContext.Session, user.UserID, user.Role);

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
        /// Logs out the current user by clearing their session.
        /// </summary>
        /// <response code="200">Logged out successfully.</response>
        [HttpPost("logout")]
        [ProducesResponseType(200)]
        public IActionResult Logout()
        {
            SessionHelper.Clear(HttpContext.Session);
            return Ok(new { message = "Logged out successfully." });
        }
    }
}