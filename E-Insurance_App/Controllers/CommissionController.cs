using Microsoft.AspNetCore.Mvc;
using E_Insurance_App.Filters;

namespace E_Insurance_App.Controllers
{
    [AuthorizeRole("Admin")]
    public class CommissionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Calculate(int agentId)
        {
            return View();
        }
    }
}
