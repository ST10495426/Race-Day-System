using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RaceDay_Api.Services;

namespace RaceDay_Api.Filters
{
    // Custom attribute that checks if the logged-in user has a specific role.
    // Usage: [RoleAuthorize("Organiser")] on a controller or action.
    // Returns 401 if not logged in, 403 if logged in with wrong role.
    public class RoleAuthorizeAttribute  : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public RoleAuthorizeAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 1. Check if user is logged in
            var userId = SessionHelper.GetUserId(context.HttpContext.Session);
            if (userId == null)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "You must be logged in." });
                return;
            }

            // 2. Check if user's role is allowed
            var role = SessionHelper.GetUserRole(context.HttpContext.Session);
            if (role == null || !_allowedRoles.Contains(role))
            {
                context.Result = new ObjectResult(new { message = "You do not have permission to access this resource." })
                {
                    StatusCode = 403
                };
            }
        }
    }
}