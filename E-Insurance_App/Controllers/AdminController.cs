using Microsoft.AspNetCore.Mvc;
using E_Insurance_App.Filters;

namespace E_Insurance_App.Controllers
{
    [AuthorizeRole("Admin")]
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
