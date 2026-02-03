using Microsoft.AspNetCore.Mvc;
using E_Insurance_App.Filters;

namespace E_Insurance_App.Controllers
{
    public class PolicyController : Controller
    {
        // Any logged-in user
        [AuthorizeRole]
        public IActionResult MyPolicies()
        {
            return View();
        }

        // Only Customer
        [AuthorizeRole("Customer")]
        public IActionResult Purchase()
        {
            return View();
        }

        // Admin only
        [AuthorizeRole("Admin")]
        public IActionResult ManagePolicies()
        {
            return View();
        }
    }
}
