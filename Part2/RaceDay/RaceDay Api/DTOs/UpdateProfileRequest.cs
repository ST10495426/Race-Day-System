using System.ComponentModel.DataAnnotations;

namespace RaceDay_Api.DTOs
{
    // Defines what the client sends when updating their profile
    public class UpdateProfileRequest
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}