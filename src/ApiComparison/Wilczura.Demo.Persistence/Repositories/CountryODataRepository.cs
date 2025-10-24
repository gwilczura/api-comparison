using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Wilczura.Demo.Persistence.Models;

namespace Wilczura.Demo.Persistence.Repositories;

public class CountryODataRepository : ODataRepository<Country>
{
    private readonly DemoDbContext _context;

    protected override DbSet<Country> DataSet => _context.Countries;

    public CountryODataRepository(
        DemoDbContext context)
    {
        _context = context;
    }

    protected override void AdjustEntityAttributes(Country entity, Country model)
    {
        entity.Code = model.Code;
        entity.Name = model.Name;
    }

    protected override Expression<Func<Country, bool>> GetEntityKeyPredicate(long key)
    {
        return (a) => a.CountryId == key;
    }

    protected override async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
