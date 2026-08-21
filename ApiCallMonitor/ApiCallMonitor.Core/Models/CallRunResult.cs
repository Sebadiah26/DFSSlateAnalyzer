namespace ApiCallMonitor.Core.Models;

/// <summary>The outcome of one call within a run. Name/Method/Url are snapshotted from the
/// <see cref="ApiCallDefinition"/> at execution time so a result still reads correctly even if the
/// underlying call definition is later edited or deleted.</summary>
public class CallRunResult
{
    public int Id { get; set; }

    public int CallRunId { get; set; }

    public CallRun? CallRun { get; set; }

    /// <summary>Null if the source call definition has since been deleted.</summary>
    public int? ApiCallDefinitionId { get; set; }

    public int Order { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Method { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    /// <summary>Null when the request never got a response at all (timeout, DNS failure, etc.) -
    /// see <see cref="ErrorMessage"/> for why.</summary>
    public int? StatusCode { get; set; }

    public bool IsSuccess { get; set; }

    public long DurationMs { get; set; }

    /// <summary>Response body, truncated to a few KB so a huge payload doesn't bloat the database.</summary>
    public string? ResponseSnippet { get; set; }

    /// <summary>Set when the request could not be completed (exception/timeout) rather than when it
    /// merely returned an unexpected status code.</summary>
    public string? ErrorMessage { get; set; }

    public DateTime ExecutedAtUtc { get; set; }
}
