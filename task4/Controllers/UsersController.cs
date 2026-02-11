using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using table.Models;

namespace table.Controllers
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
            try
            {
                foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.Status = UserStatus.Blocked;
                    await _userManager.UpdateAsync(user);
                }
            }
                TempData["Success"] = "Selected users have been blocked.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Error blocking selected users.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Unblock(string[] ids)
        {
            try
            {
                foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.Status = UserStatus.Active;
                    await _userManager.UpdateAsync(user);
                }
            }
                TempData["Success"] = "Selected users have been unblocked.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Error unblocking selected users.";
                return RedirectToAction(nameof(Index));
            }
        }


        [HttpPost]
        public async Task<IActionResult> Delete(string[] ids)
        {
            try
            {
                foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }
                TempData["Success"] = "Selected users have been deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Error deleting selected users.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUnverified(string[] ids)
        {
            // If specific ids are passed, delete only those unverified users
            if (ids != null && ids.Length > 0)
            {
                foreach (var id in ids)
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if (user != null && user.Status == UserStatus.Unverified)
                    {
                        await _userManager.DeleteAsync(user);
                    }
                }

                TempData["Success"] = "Selected unverified users have been deleted.";
                return RedirectToAction(nameof(Index));
            }

            // Otherwise, delete all unverified users (legacy behavior)
            var users = _userManager.Users
                .Where(u => u.Status == UserStatus.Unverified)
                .ToList();

            foreach (var user in users)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
