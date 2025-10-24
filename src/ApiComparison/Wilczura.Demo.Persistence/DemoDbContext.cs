using Microsoft.EntityFrameworkCore;
using Wilczura.Demo.Persistence.Models;

namespace Wilczura.Demo.Persistence;

public class DemoDbContext(DbContextOptions<DemoDbContext> options) : DbContext(options)
{
    public DbSet<CompanyLocation> CompanyLocations { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("demo");

        base.OnModelCreating(modelBuilder);
    }
}
