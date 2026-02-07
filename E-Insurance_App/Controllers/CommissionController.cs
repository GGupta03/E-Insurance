using Microsoft.AspNetCore.Mvc;
using E_Insurance_App.Filters;
using E_Insurance_App.Models.ViewModels;
using E_Insurance_App.Services.Implementations;
using E_Insurance_App.Repositories;

namespace E_Insurance_App.Controllers
{
    [AuthorizeRole("Admin")]
    public class CommissionController : Controller
    {
        private readonly CommissionService _commissionService;
        private readonly CommissionRepository _commissionRepo;

        public CommissionController(
            CommissionService commissionService,
            CommissionRepository commissionRepo)
        {
            _commissionService = commissionService;
            _commissionRepo = commissionRepo;
        }

        // List all agents with commission summary
        [HttpGet]
        public IActionResult Index()
        {
            var model = new CommissionViewModel
            {
                Agents = _commissionService.GetAllAgentsWithCommission(),
                Commissions = _commissionService.GetAllCommissions()
            };

            return View(model);
        }

        // View agent's commission details
        [HttpGet]
        public IActionResult Details(int id)
        {
            var summary = _commissionService.GetAgentCommissionSummary(id);
            
            if (summary == null)
            {
                TempData["ErrorMessage"] = "Agent not found";
                return RedirectToAction("Index");
            }

            return View(summary);
        }

        // Calculate/Assign Commission - GET
        [HttpGet]
        public IActionResult Calculate()
        {
            var model = new CalculateCommissionViewModel
            {
                Agents = _commissionService.GetAllAgentsWithCommission(),
                Policies = _commissionService.GetAvailablePolicies()
            };

            return View(model);
        }

        // Calculate/Assign Commission - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Calculate(CalculateCommissionViewModel model)
        {
            model.Agents = _commissionService.GetAllAgentsWithCommission();
            model.Policies = _commissionService.GetAvailablePolicies();

            ModelState.Remove("Agents");
            ModelState.Remove("Policies");
            ModelState.Remove("PolicyPremium");
            ModelState.Remove("CalculatedCommission");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, message) = _commissionService.ProcessCommission(
                model.AgentId,
                model.CustomerPolicyId,
                model.CommissionRate
            );

            if (success)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction("Details", new { id = model.AgentId });
            }

            TempData["ErrorMessage"] = message;
            return View(model);
        }

        // API endpoint to get policy premium (for AJAX)
        [HttpGet]
        public IActionResult GetPolicyPremium(int customerPolicyId)
        {
            var premium = _commissionService.GetPolicyPremium(customerPolicyId);
            return Json(new { premium });
        }

        // API endpoint to calculate commission preview (for AJAX)
        [HttpGet]
        public IActionResult PreviewCommission(int customerPolicyId, decimal rate)
        {
            var premium = _commissionService.GetPolicyPremium(customerPolicyId);
            var commission = premium * (rate / 100);
            return Json(new { premium, commission = Math.Round(commission, 2) });
        }
    }
}
