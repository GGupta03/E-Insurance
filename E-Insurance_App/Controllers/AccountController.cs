using Microsoft.AspNetCore.Mvc;

namespace E_Insurance_App.Controllers
{
    public class AccountController : Controller
    {

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}
