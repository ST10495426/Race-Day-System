using RaceDay_Api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_Api.Models
{
    // The Result class represents a Participant's outcome after an event.
    // Only one result can exist per enrolment (one-to-one relationship).
    public class Result
    {
        // Primary key for the Result table
        [Key]
        public int ResultID { get; set; }

        // Foreign key linking to the Enrolment this result belongs to
        // This is unique because one enrolment can only have one result
        [Required]
        public int EnrolmentID { get; set; }

        // Navigation property - lets us access the enrolment
        [ForeignKey(nameof(EnrolmentID))]
        public Enrolment? Enrolment { get; set; }

        // The time the participant finished the event (e.g. 07:45:23)
        // This is nullable because participants might not finish (DNF, DNS)
        public TimeSpan? FinishTime { get; set; }

        // The participant's finishing position (e.g. 47th place)
        // This is nullable for the same reason
        public int? Position { get; set; }

        // Status of the result: "Completed", "DNF", "DQ", or "DNS"
        // DNF = Did Not Finish
        // DQ = Disqualified
        // DNS = Did Not Start
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Completed";
    }
}