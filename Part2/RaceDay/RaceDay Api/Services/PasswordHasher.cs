using BCrypt.Net;

namespace RaceDay_Api.Services
{
    // This service handles password hashing and verification using BCrypt.
    // We NEVER store plain text passwords - only hashed versions.
    public class PasswordHasher
    {
        // Hash a plain text password
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Verify a plain text password against a stored hash
        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}