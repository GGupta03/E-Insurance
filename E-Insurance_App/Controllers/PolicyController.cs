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

        // ===============================
        // ADMIN — Manage Policies
        // ===============================
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

        // ===============================
        // ADMIN — Create Policy
        // ===============================
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

            bool created =
                _policyRepository.CreatePolicy(policy);

            if (!created)
            {
                ModelState.AddModelError("", "Unable to create policy");
                return View(model);
            }

            return RedirectToAction("ManagePolicies");
        }

        // ===============================
        // ADMIN — Edit Policy
        // ===============================
        [AuthorizeRole("Admin")]
        public IActionResult Edit(int id)
        {
            var policy =
                _policyRepository.GetPolicyById(id);

            if (policy == null)
                return NotFound();

            var model = new EditPolicyViewModel
            {
                PolicyTypeId = policy.PolicyTypeId,
                PolicyName = policy.PolicyName,
                PolicyCategory = policy.PolicyCategory,
                BasePremium = policy.BasePremium,
                InterestRate = policy.InterestRate,
                MinAge = policy.MinAge,
                MaxAge = policy.MaxAge,
                TermYears = policy.TermYears,
                IsActive = policy.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public IActionResult Edit(EditPolicyViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var policy = new PolicyType
            {
                PolicyTypeId = model.PolicyTypeId,
                PolicyName = model.PolicyName,
                PolicyCategory = model.PolicyCategory,
                BasePremium = model.BasePremium,
                InterestRate = model.InterestRate,
                MinAge = model.MinAge,
                MaxAge = model.MaxAge,
                TermYears = model.TermYears,
                IsActive = model.IsActive
            };

            _policyRepository.UpdatePolicy(policy);

            return RedirectToAction("ManagePolicies");
        }

        // ===============================
        // ADMIN — Activate/Deactivate
        // ===============================
        [HttpPost]
        [AuthorizeRole("Admin")]
        public IActionResult ToggleStatus(int id)
        {
            var policy =
                _policyRepository.GetPolicyById(id);

            if (policy == null)
                return NotFound();

            policy.IsActive = !policy.IsActive;

            _policyRepository.UpdatePolicy(policy);

            return RedirectToAction("ManagePolicies");
        }

        // ===============================
        // CUSTOMER — View Available Policies
        // ===============================
        [AuthorizeRole("Customer")]
        public IActionResult Available()
        {
            var policies =
                _policyRepository.GetActivePolicies();

            return View(policies);
        }

        // ===============================
        // CUSTOMER — Purchase Policy
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Customer")]
        public IActionResult PurchaseConfirm(int id)
        {
            int userId =
                HttpContext.Session.GetInt32("UserId").Value;

            var policy =
                _policyRepository.GetPolicyById(id);

            if (policy == null)
                return NotFound();

            bool success =
                _policyRepository.PurchasePolicy(userId, policy);

            if (!success)
                return BadRequest();

            TempData["Success"] =
                "Policy purchased successfully!";

            return RedirectToAction("Available");
        }

        // ===============================
        // CUSTOMER — My Policies Dashboard
        // ===============================
        [AuthorizeRole("Customer")]
        public IActionResult MyPolicies()
        {
            int userId =
                HttpContext.Session.GetInt32("UserId").Value;

            var policies =
                _policyRepository.GetCustomerPolicies(userId);

            return View(policies);
        }

        // CUSTOMER — Purchase screen
        [AuthorizeRole("Customer")]
        public IActionResult Purchase(int id)
        {
            var policy = _policyRepository.GetPolicyById(id);

            if (policy == null)
                return NotFound();

            return View(policy);
        }

    }
}
