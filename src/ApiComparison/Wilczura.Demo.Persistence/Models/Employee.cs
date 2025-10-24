using System.ComponentModel.DataAnnotations.Schema;

namespace Wilczura.Demo.Persistence.Models;

public class Employee
{
    public long EmployeeId { get; set; }

    public CompanyLocation? CompanyLocation { get; set; } = null!;

    public long? CompanyLocationId { get; set; }

    [Column(TypeName = "citext")]
    public string Email { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string Name { get; set; } = string.Empty;
}
