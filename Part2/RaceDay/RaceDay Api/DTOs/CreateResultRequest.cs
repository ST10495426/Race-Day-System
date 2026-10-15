using System.ComponentModel.DataAnnotations;

namespace RaceDay_Api.DTOs
{
    public class CreateResultRequest
    {
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }

        [Required]
        public string Status { get; set; } = "Completed";
    }
}