using BankSystem.Web.Data;
using BankSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BankSystem.Web.Controllers
{
    [Authorize]
    [Authorize(Roles = "Customer")]
    public class CardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

        public CardController(ApplicationDbContext context, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> RequestCard()
        {
            var userId = _userManager.GetUserId(User);
            var customer = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.Customers.Include(c => c.Accounts), c => c.UserId == userId);
            ViewBag.Accounts = customer?.Accounts ?? new System.Collections.Generic.List<Account>();
            return View(new Card());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> RequestCard(Card model)
        {
            if (ModelState.IsValid)
            {
                model.CardNumber = new System.Random().Next(10000000, 99999999).ToString() + new System.Random().Next(10000000, 99999999).ToString();
                model.CVV = new System.Random().Next(100, 999).ToString();
                model.ExpiryDate = System.DateTime.UtcNow.AddYears(4);
                
                _context.Cards.Add(model);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Card requested successfully.";
                return RedirectToAction("Dashboard", "Customer");
            }
            return View(model);
        }
    }
}
