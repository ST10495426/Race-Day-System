using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using RaceDay_Api.Data;
using RaceDay_Api.Controllers;
using RaceDay_Api.DTOs;
using RaceDay_Api.Filters;
using RaceDay_Api.Models;
using RaceDay_Api_Tests.Helpers;
using Microsoft.AspNetCore.Routing;
using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaceDay_Api_Tests
{
    /// <summary>
    /// Tests for event management including role-based access control.
    /// </summary>
    public class EventControllerTests
    {
        private EventController CreateControllerWithSession(out RaceDayDbContext context, int? userId, string? role)
        {
            context = TestDbContextFactory.Create();

            // Seed event types
            context.EventTypes.AddRange(
                new EventType { Name = "Run", Description = "Running" },
                new EventType { Name = "Cycle", Description = "Cycling" }
            );
            context.SaveChanges();

            var controller = new EventController(context);

            // Set up HttpContext with mock session
            var httpContext = new DefaultHttpContext();
            httpContext.Session = userId.HasValue
                ? SessionMock.CreateWithUser(userId.Value, role ?? "Participant")
                : SessionMock.CreateEmpty();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            return controller;
        }

        // ============================================
        // PUBLIC ENDPOINT TESTS
        // ============================================

        [Fact]
        public async Task GetAll_ReturnsAllEvents()
        {
            var controller = CreateControllerWithSession(out var context, null, null);

            context.Events.Add(new Event
            {
                OrganiserID = 1,
                EventTypeID = 1,
                Name = "Test Event",
                Date = DateTime.Now.AddDays(30),
                Location = "Test",
                Distance = "10km"
            });
            await context.SaveChangesAsync();

            var result = await controller.GetAll();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_WithInvalidId_Returns404()
        {
            var controller = CreateControllerWithSession(out _, null, null);

            var result = await controller.GetById(9999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        // ============================================
        // CREATE EVENT TESTS (ORGANISER ONLY)
        // ============================================

        [Fact]
        public async Task Create_AsOrganiser_Returns201()
        {
            var controller = CreateControllerWithSession(out _, 1, "Organiser");

            var request = new CreateEventRequest
            {
                Name = "Test Event",
                Description = "Test",
                Date = DateTime.Now.AddDays(30),
                Location = "Durban",
                Distance = "42km",
                EventTypeID = 1
            };

            var result = await controller.Create(request);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Create_WithInvalidEventType_Returns400()
        {
            var controller = CreateControllerWithSession(out _, 1, "Organiser");

            var request = new CreateEventRequest
            {
                Name = "Test Event",
                Date = DateTime.Now.AddDays(30),
                Location = "Test",
                Distance = "10km",
                EventTypeID = 9999  // Invalid
            };

            var result = await controller.Create(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // ============================================
        // ROLE ENFORCEMENT TESTS
        // ============================================

        [Fact]
        public void RoleAuthorize_NotLoggedIn_Returns401()
        {
            // User not logged in (empty session)
            var httpContext = new DefaultHttpContext { Session = SessionMock.CreateEmpty() };

            var filterContext = CreateFilterContext(httpContext);
            var filter = new RoleAuthorizeAttribute("Organiser");

            filter.OnAuthorization(filterContext);

            Assert.IsType<UnauthorizedObjectResult>(filterContext.Result);
        }

        [Fact]
        public void RoleAuthorize_ParticipantTryingOrganiserAction_Returns403()
        {
            // Logged in as Participant
            var httpContext = new DefaultHttpContext
            {
                Session = SessionMock.CreateWithUser(1, "Participant")
            };

            var filterContext = CreateFilterContext(httpContext);
            var filter = new RoleAuthorizeAttribute("Organiser");

            filter.OnAuthorization(filterContext);

            var result = Assert.IsType<ObjectResult>(filterContext.Result);
            Assert.Equal(403, result.StatusCode);
        }

        [Fact]
        public void RoleAuthorize_OrganiserAccessingOrganiserAction_Allows()
        {
            var httpContext = new DefaultHttpContext
            {
                Session = SessionMock.CreateWithUser(1, "Organiser")
            };

            var filterContext = CreateFilterContext(httpContext);
            var filter = new RoleAuthorizeAttribute("Organiser");

            filter.OnAuthorization(filterContext);

            Assert.Null(filterContext.Result);  // No rejection means allowed
        }

        // Helper to build AuthorizationFilterContext
        private static AuthorizationFilterContext CreateFilterContext(HttpContext httpContext)
        {
            var actionContext = new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor());
            return new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        }
    }
}