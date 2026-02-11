using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using table.Models;

namespace task4.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
            public class InputModel
        {
            [Required]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.Name = Input.Name;
                user.Status = UserStatus.Unverified;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // set LastLoginTime to registration time so first login is recorded
                    try
                    {
                        user.LastLoginTime = user.RegistrationTime;
                        await _userManager.UpdateAsync(user);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Unable to set LastLoginTime for newly registered user {Email}", Input.Email);
                    }

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    // Send confirmation email synchronously (await) to ensure delivery attempt is made
                    try
                    {
                        var encodedUrl = HtmlEncoder.Default.Encode(callbackUrl);
                        // use configured site name as header/logo text
                        var siteName = (string)(this.HttpContext.RequestServices.GetService(typeof(Microsoft.Extensions.Configuration.IConfiguration)) is Microsoft.Extensions.Configuration.IConfiguration cfg
                            ? cfg["Site:Name"] ?? "Task4"
                            : "Task4");
                        var encSite = HtmlEncoder.Default.Encode(siteName);
                        var encName = HtmlEncoder.Default.Encode(Input.Name);
                        var encUrl = encodedUrl;
                        var subject = "Confirm your email address";
                        var htmlMessage =
                            "<!doctype html>" +
                            "<html>" +
                            "<head>" +
                            "<meta charset=\"UTF-8\">" +
                            "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
                            "</head>" +
                            "<body style=\"margin:0;padding:0;font-family:Arial,Helvetica,sans-serif;background-color:#f4f6f8;\">" +
                            "<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\">" +
                            "<tr><td align=\"center\">" +
                            "<table role=\"presentation\" width=\"600\" style=\"max-width:600px;margin:16px;\">" +
                            "<tr><td style=\"background:#0d6efd;padding:20px;text-align:center;color:#ffffff;border-top-left-radius:8px;border-top-right-radius:8px;\">" +
                            "<div style=\"font-size:22px;font-weight:700;\">" + encSite + "</div>" +
                            "</td></tr>" +
                            "<tr><td style=\"background:#ffffff;padding:24px;border:1px solid #e9ecef;border-bottom-left-radius:8px;border-bottom-right-radius:8px;\">" +
                            "<p style=\"margin:0 0 12px 0;\">Hi " + encName + ",</p>" +
                            "<p style=\"margin:0 0 18px 0;color:#555;line-height:1.5;\">Thanks for creating an account with " + encSite + ". Please confirm your email address to activate your account and get full access.</p>" +
                            "<div style=\"text-align:center;margin:18px 0;\">" +
                            "<a href=\"" + encUrl + "\" style=\"display:inline-block;padding:12px 20px;background:#0d6efd;color:#ffffff;text-decoration:none;border-radius:6px;font-weight:600;\">Confirm your email</a>" +
                            "</div>" +
                            "<p style=\"word-break:break-word;color:#777;font-size:13px;\">If the button above does not work, copy and paste the link below into your browser:</p>" +
                            "<p style=\"word-break:break-word;font-size:12px;color:#0d6efd;\"><a href=\"" + encUrl + "\">" + encUrl + "</a></p>" +
                            "<hr style=\"border:none;border-top:1px solid #eee;margin:18px 0;\" />" +
                            "<p style=\"color:#999;font-size:12px;margin:0;\">If you didn't create this account, you can safely ignore this email.</p>" +
                            "</td></tr>" +
                            "<tr><td style=\"text-align:center;padding:12px 0 24px 0;color:#999;font-size:12px;\">&copy; " + DateTime.UtcNow.Year + " " + encSite + ". All rights reserved.</td></tr>" +
                            "</table>" +
                            "</td></tr>" +
                            "</table>" +
                            "</body></html>";

                        // attempt to send email and await result so failures are observed and logged
                        await _emailSender.SendEmailAsync(Input.Email, subject, htmlMessage);
                    }
                    catch (Exception ex)
                    {
                        // log and continue registration flow even if email fails
                        _logger.LogError(ex, "Error sending confirmation email (important)");
                    }

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
