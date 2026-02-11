using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using table.Models;

namespace table.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Redirect to users list (Razor Pages project uses Users controller as main page)
            return RedirectToAction("Index", "Users");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
