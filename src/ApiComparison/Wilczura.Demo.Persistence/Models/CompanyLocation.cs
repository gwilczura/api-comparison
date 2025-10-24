using System.ComponentModel.DataAnnotations.Schema;

namespace Wilczura.Demo.Persistence.Models;

public class CompanyLocation
{
    public long CompanyLocationId { get; set; }

    public Country Country { get; set; } = null!;

    public long CountryId { get; set; }

    [Column(TypeName = "citext")]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string Description { get; set; } = string.Empty;
}
