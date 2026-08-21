using System.Text.Json.Nodes;
using ApiCallMonitor.Core.Models;

namespace ApiCallMonitor.Core.IncidentIq;

/// <summary>Fetches and saves one Incident IQ user record at a time - the "grab a user, edit some
/// fields, post it back" workflow, as opposed to the generic collection/call model used elsewhere
/// in this app.</summary>
public interface IIncidentIqUserClient
{
    Task<IncidentIqFetchResult> GetUserAsync(IncidentIqConnectionSettings settings, string userId, CancellationToken cancellationToken = default);

    /// <summary>POSTs <paramref name="updatedUser"/> back as-is. Callers must send the full user
    /// object (see <see cref="JsonRecordEditor"/>'s doc comment) - this method doesn't merge or
    /// validate anything itself.</summary>
    Task<IncidentIqSaveResult> SaveUserAsync(IncidentIqConnectionSettings settings, string userId, JsonObject updatedUser, CancellationToken cancellationToken = default);
}

public class IncidentIqFetchResult
{
    public bool Success { get; init; }

    public int? StatusCode { get; init; }

    public JsonObject? User { get; init; }

    public string? RawResponse { get; init; }

    public string? ErrorMessage { get; init; }
}

public class IncidentIqSaveResult
{
    public bool Success { get; init; }

    public int? StatusCode { get; init; }

    public string? ResponseSnippet { get; init; }

    public string? ErrorMessage { get; init; }
}
