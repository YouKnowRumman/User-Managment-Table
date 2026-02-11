using Microsoft.AspNetCore.Identity;
using table.Models;

public class BlockedUserMiddleware
{
    private readonly RequestDelegate _next;

    public BlockedUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        // Exclude static files and Identity endpoints (Login/Register/Confirm) from middleware
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/lib") ||
            path.Contains("/Account/Login") || path.Contains("/Account/Register") || path.Contains("/Identity/Account") ||
            path.Contains("/Identity/Account/Login") || path.Contains("/Identity/Account/Register") ||
            path.Contains("/Identity/Account/ConfirmEmail") || path.Contains("/swagger"))
        {
            await _next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user == null)
            {
                // note: user missing -> treat as deleted and force sign out
                await signInManager.SignOutAsync();
                context.Response.Redirect("/Identity/Account/Login");
                return;
            }

            if (user.Status == UserStatus.Blocked)
            {
                // important: blocked users must be signed out and redirected to login
                await signInManager.SignOutAsync();
                context.Response.Redirect("/Identity/Account/Login");
                return;
            }
        }

        await _next(context);
    }
}
