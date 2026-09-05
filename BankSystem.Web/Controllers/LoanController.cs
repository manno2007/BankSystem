using BankSystem.Web.Models;
using BankSystem.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankSystem.Web.Controllers
{
    [Authorize]
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoanController(ILoanService loanService, UserManager<ApplicationUser> userManager)
        {
            _loanService = loanService;
            _userManager = userManager;
        }

        [Authorize(Roles = "Customer")]
        public IActionResult Apply() => View();

        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(Loan model)
        {
            var userId = _userManager.GetUserId(User);
            
            if (ModelState.IsValid)
            {
                var success = await _loanService.ApplyForLoanAsync(userId, model);
                if (success)
                {
                    return RedirectToAction("Dashboard", "Customer");
                }
            }
            return View(model);
        }
    }
}
