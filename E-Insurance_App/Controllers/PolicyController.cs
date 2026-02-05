using E_Insurance_App.Filters;
using E_Insurance_App.Helpers;
using E_Insurance_App.Models.Entities;
using E_Insurance_App.Models.ViewModels;
using E_Insurance_App.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace E_Insurance_App.Controllers
{
    public class PolicyController : Controller
    {
        private readonly PolicyRepository _policyRepository;

        public PolicyController(PolicyRepository policyRepository)
        {
            _policyRepository = policyRepository;
        }

        [AuthorizeRole("Admin")]
        public IActionResult ManagePolicies(int page = 1)
        {
            int size = 5;

            var policies =
                _policyRepository.GetPoliciesPaged(page, size);

            int total =
                _policyRepository.GetTotalPoliciesCount();

            var model = new PaginationHelper<PolicyType>
            {
                Items = policies,
                CurrentPage = page,
                PageSize = size,
                TotalRecords = total
            };

            return View(model);
        }

        [AuthorizeRole("Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public IActionResult Create(CreatePolicyViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var policy = new PolicyType
            {
                PolicyName = model.PolicyName,
                PolicyCategory = model.PolicyCategory,
                BasePremium = model.BasePremium,
                InterestRate = model.InterestRate,
                MinAge = model.MinAge,
                MaxAge = model.MaxAge,
                TermYears = model.TermYears,
                IsActive = true
            };

            bool created = _policyRepository.CreatePolicy(policy);

            if (!created)
            {
                ModelState.AddModelError("", "Unable to create policy");
                return View(model);
            }

            return RedirectToAction("ManagePolicies");
        }


        // Any logged-in user
        [AuthorizeRole]
        public IActionResult MyPolicies()
        {
            return View();
        }

        // Only Customer
        [AuthorizeRole("Customer")]
        public IActionResult Purchase()
        {
            return View();
        }
    }
}
