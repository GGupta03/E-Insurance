using Microsoft.AspNetCore.Mvc;
using E_Insurance_App.Filters;
using E_Insurance_App.Models.ViewModels;
using E_Insurance_App.Repositories;
using E_Insurance_App.Services.Implementations;

namespace E_Insurance_App.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentRepository _paymentRepo;
        private readonly PaymentService _paymentService;
        private readonly PolicyRepository _policyRepo;

        public PaymentController(
            PaymentRepository paymentRepo,
            PaymentService paymentService,
            PolicyRepository policyRepo)
        {
            _paymentRepo = paymentRepo;
            _paymentService = paymentService;
            _policyRepo = policyRepo;
        }

        // Payment History
        [AuthorizeRole]
        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var payments = _paymentService.GetUserPayments(userId.Value);

            var model = new PaymentViewModel
            {
                PaymentHistory = payments.Select(p => new PaymentHistoryItem
                {
                    PaymentId = p.PaymentId,
                    PolicyName = p.PolicyName ?? "N/A",
                    Amount = p.AmountPaid,
                    PaymentDate = p.PaymentDate,
                    PaymentMethod = p.PaymentMode,
                    TransactionId = p.TransactionRef ?? "",
                    Status = p.PaymentStatus ?? "Unknown"
                }).ToList()
            };

            return View(model);
        }

        // Make Payment - GET
        [AuthorizeRole]
        [HttpGet]
        public IActionResult MakePayment(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var customerPolicy = _paymentRepo.GetCustomerPolicyById(id);
            
            if (customerPolicy == null || customerPolicy.UserId != userId.Value)
            {
                TempData["ErrorMessage"] = "Policy not found or access denied";
                return RedirectToAction("MyPolicies", "Policy");
            }

            var policy = _policyRepo.GetPolicyById(customerPolicy.PolicyTypeId);

            var model = new MakePaymentViewModel
            {
                CustomerPolicyId = id,
                PolicyName = policy?.PolicyName ?? "Unknown Policy",
                Amount = customerPolicy.PremiumAmount
            };

            return View(model);
        }

        // Make Payment - POST
        [AuthorizeRole]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakePayment(MakePaymentViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customerPolicy = _paymentRepo.GetCustomerPolicyById(model.CustomerPolicyId);
            
            if (customerPolicy == null || customerPolicy.UserId != userId.Value)
            {
                TempData["ErrorMessage"] = "Policy not found or access denied";
                return RedirectToAction("MyPolicies", "Policy");
            }

            var (success, message, paymentId) = _paymentService.ProcessPayment(
                model.CustomerPolicyId,
                model.Amount,
                model.PaymentMethod
            );

            if (success)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction("Invoice", new { id = paymentId });
            }

            TempData["ErrorMessage"] = message;
            return View(model);
        }

        // View Invoice
        [AuthorizeRole]
        [HttpGet]
        public IActionResult Invoice(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var invoice = _paymentService.GetInvoice(id);

            if (invoice == null)
            {
                TempData["ErrorMessage"] = "Invoice not found";
                return RedirectToAction("Index");
            }

            return View(invoice);
        }
    }
}
