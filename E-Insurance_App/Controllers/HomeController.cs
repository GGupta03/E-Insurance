using E_Insurance_App.Filters;
using E_Insurance_App.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace E_Insurance_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserRepository _userRepository;

        public HomeController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeRole]
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _userRepository.GetUserById(userId.Value);
            if (user == null)
                return RedirectToAction("Login", "Account");

            return View(user);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
