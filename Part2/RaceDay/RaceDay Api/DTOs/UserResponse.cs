namespace RaceDay_Api.DTOs
{
    // Defines what the API sends back to the client about a user
    // NEVER includes the password hash
    public class UserResponse
    {
        public int UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}