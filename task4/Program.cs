using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using table.Models;
using task4.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Database ---
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? throw new Exception("DB_HOST not set");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? throw new Exception("DB_NAME not set");
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? throw new Exception("DB_USER not set");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? throw new Exception("DB_PASSWORD not set");

var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --- Identity ---
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 1;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// --- SMTP Email ---
var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com";
var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? throw new Exception("SMTP_USER not set");
var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new Exception("SMTP_PASSWORD not set");

builder.Services.Configure<SmtpOptions>(options =>
{
    options.Host = smtpHost;
    options.Port = smtpPort;
    options.User = smtpUser;
    options.Password = smtpPassword;
    options.EnableSsl = true;
});
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailSender>();

// --- Middleware & Routing ---
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<BlockedUserMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
