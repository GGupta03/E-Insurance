using E_Insurance_App.Filters;
using E_Insurance_App.Models.ViewModels;
using E_Insurance_App.Repositories;
using E_Insurance_App.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace E_Insurance_App.Controllers
{
    [AuthorizeRole]
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
        public IActionResult Calculate(PremiumViewModel model)
        {
            model.Policies =
                _policyRepo.GetActivePolicies();

            if (!ModelState.IsValid)
                return View(model);

            var policy =
                _policyRepo.GetPolicyById(model.PolicyTypeId);

            if (policy == null)
                return View(model);

            model.Result =
                _premiumService.CalculatePremium(
                    policy,
                    model.Age);

            return View(model);
        }
    }
}
