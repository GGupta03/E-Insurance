using E_Insurance_App.Filters;
using E_Insurance_App.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace E_Insurance_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserRepository _userRepo;

        public HomeController(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            var admin = _userRepo.GetUserByEmail("admin@einsurance.com");
            ViewBag.Email = admin?.Email ?? "Not Found";
            return View();
        }

        [AuthorizeRole]
        public IActionResult Profile()
        {
            return View();
        }
    }
}
