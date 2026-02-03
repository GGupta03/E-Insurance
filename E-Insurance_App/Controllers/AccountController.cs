using Microsoft.AspNetCore.Mvc;
using E_Insurance_App.Models.ViewModels;
using E_Insurance_App.Models.Entities;
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
            {
                return View(model);
            }

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

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if email already exists
            var existingUser = _userRepository.GetUserByEmail(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email is already registered");
                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = PasswordHelper.HashPassword(model.Password),
                Role = "Customer",
                IsActive = true
            };

            bool created = _userRepository.CreateUser(user);

            if (!created)
            {
                ModelState.AddModelError("", "Unable to create account. Please try again.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
