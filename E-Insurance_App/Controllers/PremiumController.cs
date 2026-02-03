using Microsoft.AspNetCore.Mvc;
using E_Insurance_App.Filters;

namespace E_Insurance_App.Controllers
{
    public class PremiumController : Controller
    {
        [AuthorizeRole]
        public IActionResult Calculate()
        {
            return View();
        }
    }
}
