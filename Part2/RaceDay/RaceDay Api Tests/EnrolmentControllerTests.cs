using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay_Api.Controllers;
using RaceDay_Api.DTOs;
using RaceDay_Api.Models;
using RaceDay_Api_Tests.Helpers;
using RaceDay_Api.Data;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaceDay_Api_Tests
{
    /// <summary>
    /// Tests for event enrolment including duplicate prevention.
    /// </summary>
    public class EnrolmentControllerTests
    {
        private EnrolmentController CreateControllerWithSession(out RaceDayDbContext context, int userId, string role)
        {
            context = TestDbContextFactory.Create();

            // Seed prerequisite data
            var organiser = new User
            {
                Email = "org@test.com",
                PasswordHash = "x",
                FullName = "Org",
                DateOfBirth = DateTime.Now,
                PhoneNumber = "1",
                Role = "Organiser"
            };
            var participant = new User
            {
                Email = "part@test.com",
                PasswordHash = "x",
                FullName = "Part",
                DateOfBirth = DateTime.Now,
                PhoneNumber = "2",
                Role = "Participant"
            };
            context.Users.AddRange(organiser, participant);
            context.EventTypes.Add(new EventType { Name = "Run" });
            context.SaveChanges();

            var ev = new Event
            {
                OrganiserID = organiser.UserID,
                EventTypeID = 1,
                Name = "Test Event",
                Date = DateTime.Now.AddDays(30),
                Location = "Test",
                Distance = "10km"
            };
            context.Events.Add(ev);
            context.SaveChanges();

            context.Categories.Add(new Category
            {
                EventID = ev.EventID,
                Name = "Senior",
                CategoryType = "Age",
                MinValue = 20,
                MaxValue = 39
            });
            context.SaveChanges();

            var controller = new EnrolmentController(context);

            var httpContext = new DefaultHttpContext
            {
                Session = SessionMock.CreateWithUser(userId, role)
            };
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            return controller;
        }

        [Fact]
        public async Task Enrol_AsParticipant_Returns201()
        {
            var controller = CreateControllerWithSession(out _, 2, "Participant");

            var result = await controller.Enrol(1, new EnrolRequest { CategoryID = 1 });

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Enrol_DuplicateEnrolment_Returns409()
        {
            var controller = CreateControllerWithSession(out _, 2, "Participant");

            await controller.Enrol(1, new EnrolRequest { CategoryID = 1 });  // First
            var result = await controller.Enrol(1, new EnrolRequest { CategoryID = 1 });  // Duplicate

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Enrol_WithInvalidCategory_Returns404()
        {
            var controller = CreateControllerWithSession(out _, 2, "Participant");

            var result = await controller.Enrol(1, new EnrolRequest { CategoryID = 9999 });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetMyEnrolments_AsParticipant_ReturnsOwnEnrolments()
        {
            var controller = CreateControllerWithSession(out _, 2, "Participant");
            await controller.Enrol(1, new EnrolRequest { CategoryID = 1 });

            var result = await controller.GetMyEnrolments();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Withdraw_AsParticipant_SetsStatusToCancelled()
        {
            var controller = CreateControllerWithSession(out var context, 2, "Participant");
            await controller.Enrol(1, new EnrolRequest { CategoryID = 1 });

            var enrolmentId = context.Enrolments.First().EnrolmentID;
            var result = await controller.Withdraw(enrolmentId);

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Cancelled", context.Enrolments.First().Status);
        }
    }
}