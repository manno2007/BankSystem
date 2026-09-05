using BankSystem.Web.Models;
using BankSystem.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankSystem.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(ICustomerService customerService, UserManager<ApplicationUser> userManager)
        {
            _customerService = customerService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var customer = await _customerService.GetCustomerByUserIdAsync(userId);

            if (customer == null)
            {
                return RedirectToAction(nameof(CreateProfile));
            }

            return View(customer);
        }

        [HttpGet]
        public IActionResult CreateProfile() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfile(Customer model)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);
            if (userId == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                var firstName = user?.FirstName ?? string.Empty;
                var lastName = user?.LastName ?? string.Empty;
                
                await _customerService.CreateCustomerProfileAsync(userId, firstName, lastName, model);
                return RedirectToAction(nameof(Dashboard));
            }
            return View(model);
        }
    }
}
