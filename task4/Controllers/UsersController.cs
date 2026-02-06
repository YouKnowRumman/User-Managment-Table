using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using task4.Models;

namespace task4.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Block(string[] ids)
        {
            foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.IsBlocked = true;
                    await _userManager.UpdateAsync(user);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Unblock(string[] ids)
        {
            foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.IsBlocked = false;
                    await _userManager.UpdateAsync(user);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string[] ids)
        {
            foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
