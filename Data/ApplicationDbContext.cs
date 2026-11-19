using CologneWorld.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CologneWorld.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(120).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(1000).IsRequired();
            entity.Property(p => p.Category).HasMaxLength(80).IsRequired();
            entity.Property(p => p.UnitPrice).HasPrecision(12, 2);
            entity.Property(p => p.ImageUrl).HasMaxLength(1000);
        });
    }
}
