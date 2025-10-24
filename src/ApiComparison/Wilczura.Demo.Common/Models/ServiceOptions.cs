namespace Wilczura.Demo.Common.Models;

public class ServiceOptions
{
    public string? ApiKey { get; set; }
    public bool EnableAutomaticMigration { get; set; }
    public string[]? Cors { get; set; }
}
