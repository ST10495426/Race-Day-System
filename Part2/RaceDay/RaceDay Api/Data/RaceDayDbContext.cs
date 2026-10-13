using Microsoft.EntityFrameworkCore;
using RaceDay_Api.Models;

namespace RaceDay_Api.Data
{
    // The DbContext is the main class that talks to the database.
    // It inherits from Entity Framework Core's DbContext.
    // Each DbSet below represents a table in the database.
    public class RaceDayDbContext : DbContext
    {
        // Constructor - receives options from Program.cs (like connection string)
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options)
            : base(options)
        {
        }

        // Each DbSet maps to a table in the database
        public DbSet<User> Users { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<Result> Results { get; set; }

        // OnModelCreating is where we configure rules EF Core can't figure out on its own.
        // This is where we enforce the constraints from our Part 1 SQL script.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================================
            // USER CONFIGURATION
            // ============================================
            // Email must be unique across all users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Role must be either "Organiser" or "Participant"
            modelBuilder.Entity<User>()
                .ToTable(t => t.HasCheckConstraint("CK_User_Role",
                    "[Role] IN ('Organiser', 'Participant')"));

            // ============================================
            // EVENTTYPE CONFIGURATION
            // ============================================
            // EventType name must be unique
            modelBuilder.Entity<EventType>()
                .HasIndex(et => et.Name)
                .IsUnique();

            // ============================================
            // EVENT CONFIGURATION
            // ============================================
            // One Organiser creates many Events
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser)
                .WithMany()
                .HasForeignKey(e => e.OrganiserID)
                .OnDelete(DeleteBehavior.Restrict);

            // One EventType has many Events
            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventType)
                .WithMany()
                .HasForeignKey(e => e.EventTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================================
            // CATEGORY CONFIGURATION
            // ============================================
            // CategoryType must be either "Age" or "Distance"
            modelBuilder.Entity<Category>()
                .ToTable(t => t.HasCheckConstraint("CK_Category_Type",
                    "[CategoryType] IN ('Age', 'Distance')"));

            // One Event has many Categories
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventID)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================================
            // ENROLMENT CONFIGURATION
            // ============================================
            // Status must be one of three values
            modelBuilder.Entity<Enrolment>()
                .ToTable(t => t.HasCheckConstraint("CK_Enrolment_Status",
                    "[Status] IN ('Pending', 'Confirmed', 'Cancelled')"));

            // One Participant has many Enrolments
            modelBuilder.Entity<Enrolment>()
                .HasOne(en => en.Participant)
                .WithMany()
                .HasForeignKey(en => en.ParticipantID)
                .OnDelete(DeleteBehavior.Restrict);

            // One Event has many Enrolments
            modelBuilder.Entity<Enrolment>()
                .HasOne(en => en.Event)
                .WithMany(e => e.Enrolments)
                .HasForeignKey(en => en.EventID)
                .OnDelete(DeleteBehavior.Restrict);

            // One Category is used in many Enrolments
            modelBuilder.Entity<Enrolment>()
                .HasOne(en => en.Category)
                .WithMany(c => c.Enrolments)
                .HasForeignKey(en => en.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint: A Participant cannot enroll in the same Event twice
            // This is the "EventParticipant" constraint from Part 1
            modelBuilder.Entity<Enrolment>()
                .HasIndex(en => new { en.ParticipantID, en.EventID })
                .IsUnique()
                .HasDatabaseName("EventParticipant");

            // ============================================
            // RESULT CONFIGURATION
            // ============================================
            // Status must be one of four values
            modelBuilder.Entity<Result>()
                .ToTable(t => t.HasCheckConstraint("CK_Result_Status",
                    "[Status] IN ('Completed', 'DNF', 'DQ', 'DNS')"));

            // One Enrolment has one Result
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(en => en.Result)
                .HasForeignKey<Result>(r => r.EnrolmentID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}