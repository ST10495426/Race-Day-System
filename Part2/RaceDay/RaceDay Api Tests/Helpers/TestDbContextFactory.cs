using Microsoft.EntityFrameworkCore;
using RaceDay_Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay_Api_Tests.Helpers
{
    // Creates an in-memory database for testing.
    // Each test gets its own isolated database.
    public static class TestDbContextFactory
    {
        public static RaceDayDbContext Create()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new RaceDayDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}