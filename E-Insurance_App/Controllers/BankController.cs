using E_Insurance_App.Filters;
using E_Insurance_App.Helpers;
using E_Insurance_App.Models.Entities;
using E_Insurance_App.Models.ViewModels;
using E_Insurance_App.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace E_Insurance_App.Controllers
{
    [AuthorizeRole("Admin")]
    public class BankController : Controller
    {
        private readonly BankRepository _bankRepository;

        public BankController(BankRepository bankRepository)
        {
            _bankRepository = bankRepository;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 5;

            var banks = _bankRepository.GetBanksPaged(page, pageSize);
            int totalBanks = _bankRepository.GetTotalBanksCount();

            var model = new PaginationHelper<Bank>
            {
                Items = banks,
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecords = totalBanks
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateBankViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var bank = new Bank
            {
                BankName = model.BankName,
                IFSC = model.IFSC,
                Branch = model.Branch,
                IsActive = true
            };

            bool created = _bankRepository.CreateBank(bank);

            if (!created)
            {
                ModelState.AddModelError("", "Unable to create bank");
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}
