using E_Insurance_App.Filters;
using E_Insurance_App.Helpers;
using E_Insurance_App.Models.Entities;
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

        // Admin only
        [AuthorizeRole("Admin")]
        public IActionResult ManagePolicies()
        {
            return View();
        }
    }
}
