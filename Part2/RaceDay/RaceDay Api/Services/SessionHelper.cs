namespace RaceDay_Api.Services
{
    // A simple helper to manage user session data.
    // Stores UserID and Role in the session.
    public class SessionHelper
    {
        // Save user info to session after login
        public static void SetUser(ISession session, int userId, string role)
        {
            session.SetInt32("UserId", userId);
            session.SetString("UserRole", role);
        }

        // Get the current user's ID (null if not logged in)
        public static int? GetUserId(ISession session)
        {
            return session.GetInt32("UserId");
        }

        // Get the current user's role (null if not logged in)
        public static string? GetUserRole(ISession session)
        {
            return session.GetString("UserRole");
        }

        // Clear session data (logout)
        public static void Clear(ISession session)
        {
            session.Clear();
        }
    }
}