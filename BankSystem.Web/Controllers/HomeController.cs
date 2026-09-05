using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
