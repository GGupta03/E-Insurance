using E_Insurance_App.Filters;
using Microsoft.AspNetCore.Mvc;

namespace E_Insurance_App.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeRole]
        public IActionResult Profile()
        {
            return View();
        }
    }
}
