using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Try environment variable first for CI/containers, else fallback to appsettings value
        var conn = Environment.GetEnvironmentVariable("DefaultConnection")
                   ?? "Host=localhost;Port=5432;Database=task4_db;Username=postgres;Password=YourPassword";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(conn);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}