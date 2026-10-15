using System.ComponentModel.DataAnnotations;

namespace RaceDay_Api.DTOs
{
    public class UpdateEventRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Distance { get; set; } = string.Empty;

        [Required]
        public int EventTypeID { get; set; }
    }
}