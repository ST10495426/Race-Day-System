using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay_Api.Controllers;
using RaceDay_Api.Data;
using RaceDay_Api.DTOs;
using RaceDay_Api.Services;
using RaceDay_Api_Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaceDay_Api_Tests
{
    /// <summary>
    /// Tests for user registration, login, and role validation.
    /// </summary>
    public class AuthControllerTests
    {
        // Helper to create a controller with a fresh database
        private AuthController CreateController(out RaceDayDbContext context)
        {
            context = TestDbContextFactory.Create();
            var hasher = new PasswordHasher();
            var controller = new AuthController(context, hasher);

            // Set up HttpContext with a mock session
            var httpContext = new DefaultHttpContext();
            httpContext.Session = SessionMock.CreateEmpty();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            return controller;
        }

        private static RegisterRequest ValidRegisterRequest(string role = "Organiser", string email = "hlatse@testraceday.co.za")
        {
            return new RegisterRequest
            {
                Email = email,
                Password = "password123",
                FullName = "Test User",
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "+27 82 123 4567",
                Role = role
            };
        }

        // ============================================
        // REGISTRATION TESTS
        // ============================================

        [Fact]
        public async Task Register_WithValidOrganiser_Returns201()
        {
            var controller = CreateController(out var context);
            var request = ValidRegisterRequest("Organiser");

            var result = await controller.Register(request);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<UserResponse>(created.Value);
            Assert.Equal("Organiser", response.Role);
            Assert.Equal("hlatse@testraceday.co.za", response.Email);
        }

        [Fact]
        public async Task Register_WithValidParticipant_Returns201()
        {
            var controller = CreateController(out _);
            var request = ValidRegisterRequest("Participant", "participant@raceday.co.za");

            var result = await controller.Register(request);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<UserResponse>(created.Value);
            Assert.Equal("Participant", response.Role);
        }

        [Fact]
        public async Task Register_WithInvalidRole_Returns400()
        {
            var controller = CreateController(out _);
            var request = ValidRegisterRequest("Admin");  // Invalid role

            var result = await controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_Returns409()
        {
            var controller = CreateController(out _);
            var request = ValidRegisterRequest();

            await controller.Register(request);  // First register
            var result = await controller.Register(request);  // Duplicate

            Assert.IsType<ConflictObjectResult>(result);
        }

        // ============================================
        // LOGIN TESTS
        // ============================================

        [Fact]
        public async Task Login_WithValidCredentials_Returns200()
        {
            var controller = CreateController(out _);
            var register = ValidRegisterRequest();
            await controller.Register(register);

            var result = await controller.Login(new LoginRequest
            {
                Email = "hlatse@testraceday.co.za",
                Password = "password123"
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UserResponse>(ok.Value);
            Assert.Equal("hlatse@testraceday.co.za", response.Email);
        }

        [Fact]
        public async Task Login_WithWrongPassword_Returns401()
        {
            var controller = CreateController(out _);
            await controller.Register(ValidRegisterRequest());

            var result = await controller.Login(new LoginRequest
            {
                Email = "hlatse@testraceday.co.za",
                Password = "wrongpassword"
            });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_WithUnknownEmail_Returns401()
        {
            var controller = CreateController(out _);

            var result = await controller.Login(new LoginRequest
            {
                Email = "nobody@raceday.co.za",
                Password = "password123"
            });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}