using Microsoft.AspNetCore.Mvc;
using E_Insurance_App.Filters;

namespace E_Insurance_App.Controllers
{
    public class UserController : Controller
    {
        // 🔐 Login required
        [AuthorizeRole]
        public IActionResult Profile()
        {
            return View();
        }
    }
}
