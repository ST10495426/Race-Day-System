using RaceDay_Api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_Api.Models
{
    public class Event
    {
       

        [Key] //this is the primary key of the table
        public int EventID { get; set; } // thus this is a unique identifier for each user

        [Required] //this field cannot be empty
        public int OrganiserID { get; set; }

        [ForeignKey(nameof(OrganiserID))]
        public User? Organiser { get; set; } // ? after string means the field can be null

        [Required]
        public int EventTypeID { get; set; }

        [ForeignKey(nameof(EventTypeID))]
        public EventType? EventType { get; set; }

        [Required]
        [MaxLength(255)] //number of acceptable characters
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(255)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Distance { get; set; } = string.Empty; // "10km", "21km", "90km"

        [MaxLength(500)]
        public string? BannerUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Category> Categories { get; set; } = new List<Category>(); //one event can have many categories
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}