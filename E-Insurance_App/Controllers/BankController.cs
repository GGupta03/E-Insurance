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
        public IActionResult Edit(int id)
        {
            var bank = _bankRepository.GetBankById(id);

            if (bank == null)
                return NotFound();

            var model = new E_Insurance_App.Models.ViewModels.EditBankViewModel
            {
                BankId = bank.BankId,
                BankName = bank.BankName,
                IFSC = bank.IFSC,
                Branch = bank.Branch,
                IsActive = bank.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(E_Insurance_App.Models.ViewModels.EditBankViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var bank = new E_Insurance_App.Models.Entities.Bank
            {
                BankId = model.BankId,
                BankName = model.BankName,
                IFSC = model.IFSC,
                Branch = model.Branch,
                IsActive = model.IsActive
            };

            bool updated = _bankRepository.UpdateBank(bank);

            if (!updated)
            {
                ModelState.AddModelError("", "Unable to update bank");
                return View(model);
            }

            return RedirectToAction("Index");
        }

    }
}
