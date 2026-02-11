using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using table.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Use the PostgreSQL provider (Npgsql)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    // allow any non-empty password (require at least 1 char) per Task #4 requirements
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 1;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// Email sender configuration (from config or environment)
builder.Services.Configure<task4.Services.SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, task4.Services.EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
// Serve static files (wwwroot) so CSS/JS (Bootstrap, site.css) are available
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
