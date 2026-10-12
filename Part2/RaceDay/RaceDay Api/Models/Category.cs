using RaceDay_Api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_Api.Models
{
    // The Category class represents an age or distance group within an event.
    // Foe example: Senior (20-39) is an age category, 10km is a distance category.
    public class Category
    {
        
        [Key] // Primary key for the Category table
        public int CategoryID { get; set; }

        // Foreign key linking the category to a specific event
        // Each category belongs to one event
        [Required]
        public int EventID { get; set; }

        // Navigation property - lets us access the event this category belongs to
        [ForeignKey(nameof(EventID))]
        public Event? Event { get; set; }

        // Name of the category (e.g. "Senior (20-39)", "10km", "Veteran (60+)")
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // The type of category: either "Age" or "Distance"
        // We use this to know whether MinValue/MaxValue are ages or distances in km
        [Required]
        [MaxLength(20)]
        public string CategoryType { get; set; } = string.Empty;

        // The minimum value for this category
        // For Age: the youngest age allowed (e.g. 20)
        // For Distance: the distance in km (e.g. 10)
        public int? MinValue { get; set; }

        // The maximum value for this category
        // For Age: the oldest age allowed (e.g. 39)
        // For Distance: the distance in km (e.g. 10)
        public int? MaxValue { get; set; }

        // Navigation property - one category can be used in many enrolments
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}