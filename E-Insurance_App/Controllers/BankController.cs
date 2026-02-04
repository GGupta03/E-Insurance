using Microsoft.AspNetCore.Mvc;
using E_Insurance_App.Filters;
using E_Insurance_App.Repositories;
using E_Insurance_App.Helpers;
using E_Insurance_App.Models.Entities;

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
    }
}
