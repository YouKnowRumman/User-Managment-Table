using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using table.Models;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Ensure database-level unique index on normalized email to guarantee uniqueness
        builder.Entity<ApplicationUser>(b =>
        {
            b.HasIndex(u => u.NormalizedEmail).IsUnique();
        });
    }

    public DbSet<table.Models.Table> Table { get; set; } = default!;

    // Helper: generate unique id value (nota bene: used for deterministic seeding or other features)
    public static string GetUniqIdValue()
    {
        // important: use GUID to guarantee uniqueness across concurrent inserts
        return Guid.NewGuid().ToString("N");
    }
}
