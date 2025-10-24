using System.ComponentModel.DataAnnotations.Schema;
using Wilczura.Demo.Common;

namespace Wilczura.Demo.Persistence.Models;

public class Country : IGetId
{
    public long CountryId { get; set; }

    [Column(TypeName = "citext")]
    public string Code { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string Name { get; set; } = string.Empty;

    public long GetId()
    {
        return CountryId;
    }
}
