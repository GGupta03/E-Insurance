using Microsoft.AspNetCore.Mvc;
using E_Insurance_App.Repositories;
using E_Insurance_App.Filters;

namespace E_Insurance_App.Controllers
{
    [AuthorizeRole("Admin")]
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: /User
        public IActionResult Index()
        {
            var users = _userRepository.GetAllUsers();
            return View(users);
        }
    }
}
