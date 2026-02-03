using E_Insurance_App.Filters;
using E_Insurance_App.Helpers;
using E_Insurance_App.Models.Entities;
using E_Insurance_App.Models.ViewModels;
using E_Insurance_App.Repositories;
using Microsoft.AspNetCore.Mvc;

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

        [AuthorizeRole("Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public IActionResult Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Role = model.Role,
                IsActive = model.IsActive,
                PasswordHash = PasswordHelper.HashPassword(model.Password)
            };

            bool created = _userRepository.CreateUser(user);

            if (!created)
            {
                ModelState.AddModelError("", "Unable to create user");
                return View(model);
            }

            return RedirectToAction("Index");
        }

    }
}
