using BankSystem.Web.Data;
using BankSystem.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BankSystem.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Statement(int accountId)
        {
            var model = new StatementViewModel { AccountId = accountId };
            model.Transactions = await _context.Transactions
                .Where(t => t.AccountId == accountId && t.TransactionDate >= model.StartDate && t.TransactionDate <= model.EndDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return View(model);
        }
    }
}
