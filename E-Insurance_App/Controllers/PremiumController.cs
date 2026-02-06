using E_Insurance_App.Filters;
using E_Insurance_App.Models.ViewModels;
using E_Insurance_App.Repositories;
using E_Insurance_App.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace E_Insurance_App.Controllers
{
    public class PremiumController : Controller
    {
        private readonly PolicyRepository _policyRepo;
        private readonly PremiumService _premiumService;

        public PremiumController(
            PolicyRepository policyRepo,
            PremiumService premiumService)
        {
            _policyRepo = policyRepo;
            _premiumService = premiumService;
        }

        [AuthorizeRole]
        [HttpGet]
        public IActionResult Calculate()
        {
            var model = new PremiumViewModel
            {
                Policies = _policyRepo.GetActivePolicies()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole]
        public IActionResult Calculate(PremiumViewModel model)
        {
            // Always reload policies for the dropdown
            model.Policies = _policyRepo.GetActivePolicies();

            // Clear validation errors for non-input properties
            ModelState.Remove("Policies");
            ModelState.Remove("SelectedPolicyName");
            ModelState.Remove("Result");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var policy = _policyRepo.GetPolicyById(model.PolicyTypeId);

            if (policy == null)
            {
                ModelState.AddModelError("PolicyTypeId", "Selected policy not found");
                return View(model);
            }

            // Validate age against policy requirements
            if (model.Age < policy.MinAge || model.Age > policy.MaxAge)
            {
                ModelState.AddModelError("Age", 
                    $"Age must be between {policy.MinAge} and {policy.MaxAge} for this policy");
                return View(model);
            }

            model.Result = _premiumService.CalculatePremium(policy, model.Age);
            model.SelectedPolicyName = policy.PolicyName;

            return View(model);
        }
    }
}
