using Wilczura.Demo.Host.OData;
using Wilczura.Demo.Persistence.Models;
using Wilczura.Demo.Persistence.Repositories;

namespace Wilczura.Demo.Host.Controllers.OData;

public class CountryController : CustomODataController<Country>
{
    public CountryController(ODataRepository<Country> repository) : base(repository)
    {
    }
}
