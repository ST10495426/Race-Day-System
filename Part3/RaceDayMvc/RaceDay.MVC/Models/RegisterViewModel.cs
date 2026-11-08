using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength (6, ErrorMessage = "Password must be at least 6 characters" )]
        [DataType (DataType. Password)]
        public string Password {get;set;} = string.Empty;

        [ Required]
        public string FullName {get;set; } = string.Empty;

        [Required ]
        [DataType (DataType.Date) ]
        public DateTime DateOfBirth { get;set;}

        [Required]
        public string PhoneNumber {get; set; } = string.Empty;

        [Required]
        public string Role { get;set; } = "Participant"; // Default to Participant
    }
}