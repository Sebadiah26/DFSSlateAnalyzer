namespace ApiCallMonitor.Core.Models;

/// <summary>A named, ordered set of API calls that get configured together and run as a unit
/// (e.g. "Nightly health checks", "Staging smoke test"). This is the top-level thing a user
/// creates, adds calls to, and clicks "Run" on.</summary>
public class ApiCallCollection
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<ApiCallDefinition> Calls { get; set; } = new();
}
