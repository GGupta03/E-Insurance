using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_Insurance_App.Filters
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            var userId = session.GetInt32("UserId");
            var userRole = session.GetString("UserRole");

            // Not logged in
            if (userId == null)
            {
                context.Result = new RedirectToActionResult(
                    "Login", "Account", null);
                return;
            }

            // Role check
            if (_roles.Length > 0 && !_roles.Contains(userRole))
            {
                context.Result = new RedirectToActionResult(
                    "AccessDenied", "Account", null);
            }
        }
    }
}
