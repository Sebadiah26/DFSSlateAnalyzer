namespace ApiCallMonitor.Core.Models;

/// <summary>Audit trail row for one user edited through the Incident IQ user editor (grab one
/// user, edit some fields, post it back) - what changed and whether Incident IQ accepted it.</summary>
public class UserEditLogEntry
{
    public int Id { get; set; }

    public string IncidentIqUserId { get; set; } = string.Empty;

    /// <summary>Best-effort display name pulled from the fetched record (Name/FullName/Email/...),
    /// purely for readability in the log - not authoritative, and not always present.</summary>
    public string? UserDisplayName { get; set; }

    /// <summary>JSON-serialized <see cref="List{T}"/> of <see cref="UserFieldChange"/> for every
    /// field that actually changed. "[]" if the record was posted back with no edits.</summary>
    public string ChangedFieldsJson { get; set; } = "[]";

    /// <summary>Null when the request never got a response at all (timeout, DNS failure, etc.).</summary>
    public int? StatusCode { get; set; }

    public bool IsSuccess { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime EditedAtUtc { get; set; } = DateTime.UtcNow;
}
