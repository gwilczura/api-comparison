using Microsoft.EntityFrameworkCore;

namespace Wilczura.Demo.Persistence;

public class DemoDbContext(DbContextOptions<DemoDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("demo");

        base.OnModelCreating(modelBuilder);
    }
}
