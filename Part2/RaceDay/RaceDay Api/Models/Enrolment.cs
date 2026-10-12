using RaceDay_Api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_Api.Models
{
    // The Enrolment class represents a Participant entering an Event.
    // It links three things together: a Participant, an Event, and a Category.
    // This is a "junction table" in database terms.
    public class Enrolment
    {
        // Primary key for the Enrolment table
        [Key]
        public int EnrolmentID { get; set; }

        // Foreign key linking to the User (must be a Participant)
        [Required]
        public int ParticipantID { get; set; }

        // Navigation property - lets us access the Participant who enrolled
        [ForeignKey(nameof(ParticipantID))]
        public User? Participant { get; set; }

        // Foreign key linking to the Event the participant enrolled in
        [Required]
        public int EventID { get; set; }

        // Navigation property - lets us access the Event
        [ForeignKey(nameof(EventID))]
        public Event? Event { get; set; }

        // Foreign key linking to the Category the participant selected
        [Required]
        public int CategoryID { get; set; }

        // Navigation property - lets us access the Category
        [ForeignKey(nameof(CategoryID))]
        public Category? Category { get; set; }

        // The date the participant enrolled - defaults to now
        public DateTime EnrolmentDate { get; set; } = DateTime.Now;

        // Status of the enrolment: "Pending", "Confirmed", or "Cancelled"
        // Required for Part 3 colour-coded statuses
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Confirmed";

        // Navigation property - one enrolment can have one result (after the event)
        public Result? Result { get; set; }
    }
}