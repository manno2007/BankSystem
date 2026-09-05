using BankSystem.Web.Models.ViewModels;
using BankSystem.Web.Services.Interfaces;
using BankSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankSystem.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionController(
            ITransactionService transactionService, 
            ICustomerService customerService, 
            UserManager<ApplicationUser> userManager)
        {
            _transactionService = transactionService;
            _customerService = customerService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Transfer()
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.Accounts = await _customerService.GetCustomerAccountsAsync(userId);
            return View(new TransferViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(TransferViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.Accounts = await _customerService.GetCustomerAccountsAsync(userId);

            if (!ModelState.IsValid) return View(model);

            var (isSuccess, errorMessage) = await _transactionService.TransferAsync(userId, model);

            if (!isSuccess)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(model);
            }

            TempData["Message"] = "Transfer successful.";
            return RedirectToAction("Dashboard", "Customer");
        }

        [HttpGet]
        public async Task<IActionResult> Deposit()
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.Accounts = await _customerService.GetCustomerAccountsAsync(userId);
            return View(new DepositViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(DepositViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.Accounts = await _customerService.GetCustomerAccountsAsync(userId);

            if (!ModelState.IsValid) return View(model);

            var (isSuccess, errorMessage) = await _transactionService.DepositAsync(userId, model);

            if (!isSuccess)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(model);
            }

            TempData["Message"] = "Deposit successful.";
            return RedirectToAction("Dashboard", "Customer");
        }

        [HttpGet]
        public async Task<IActionResult> Withdraw()
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.Accounts = await _customerService.GetCustomerAccountsAsync(userId);
            return View(new WithdrawViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(WithdrawViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.Accounts = await _customerService.GetCustomerAccountsAsync(userId);

            if (!ModelState.IsValid) return View(model);

            var (isSuccess, errorMessage) = await _transactionService.WithdrawAsync(userId, model);

            if (!isSuccess)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(model);
            }

            TempData["Message"] = "Withdrawal successful.";
            return RedirectToAction("Dashboard", "Customer");
        }
    }
}
