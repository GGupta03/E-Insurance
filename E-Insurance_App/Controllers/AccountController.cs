using Microsoft.AspNetCore.Mvc;
using E_Insurance_App.Models.ViewModels;
using E_Insurance_App.Repositories;
using E_Insurance_App.Helpers;

namespace E_Insurance_App.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;

        public AccountController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _userRepository.GetUserByEmail(model.Email);

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            bool isPasswordValid =
                PasswordHelper.VerifyPassword(model.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            // Store session data
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);

            // Role-based redirect
            return user.Role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Customer" => RedirectToAction("Index", "Home"),
                "Agent" => RedirectToAction("Index", "Home"),
                "Employee" => RedirectToAction("Index", "Home"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
