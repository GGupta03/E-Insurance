using Microsoft.AspNetCore.Mvc;
using E_Insurance_App.Filters;

namespace E_Insurance_App.Controllers
{
    public class PaymentController : Controller
    {
        [AuthorizeRole]
        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeRole]
        public IActionResult Invoice(int id)
        {
            return View();
        }
    }
}
