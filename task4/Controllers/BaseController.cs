using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class BaseController : Controller
{
    protected readonly UserManager<ApplicationUser> _userManager;

    public BaseController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override async void OnActionExecuting(ActionExecutingContext context)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.IsBlocked)
            {
                context.Result = RedirectToAction("Logout", "Account");
            }
        }
        base.OnActionExecuting(context);
    }
}
public class UsersController : BaseController
{
    public UsersController(UserManager<ApplicationUser> userManager)
        : base(userManager) { }
}
