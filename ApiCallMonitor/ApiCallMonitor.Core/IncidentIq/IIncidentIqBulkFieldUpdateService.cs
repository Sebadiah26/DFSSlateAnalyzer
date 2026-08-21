using ApiCallMonitor.Core.Models;

namespace ApiCallMonitor.Core.IncidentIq;

/// <summary>One row of a bulk update: a user id plus the field name/value pairs to set on it -
/// e.g. from a CSV of UserId,JobTitle exported from a SQL query.</summary>
public record BulkUpdateRow(string UserId, IReadOnlyDictionary<string, string> FieldValues);

/// <summary>Applies a fixed set of named field values to one Incident IQ user - the automated
/// counterpart to the interactive user editor, built on the same <see cref="JsonRecordEditor"/> so
/// both go through identical fetch-merge-post logic. Used by the bulk update page for driving many
/// rows (e.g. "1000 users' JobTitle, from a CSV") without a human clicking through each one.</summary>
public interface IIncidentIqBulkFieldUpdateService
{
    Task<BulkFieldUpdateRowResult> ApplyFieldUpdatesAsync(
        IncidentIqConnectionSettings settings,
        string userId,
        IReadOnlyDictionary<string, string> fieldValues,
        CancellationToken cancellationToken = default);
}

public class BulkFieldUpdateRowResult
{
    public required string UserId { get; init; }

    public string? DisplayName { get; init; }

    public bool FetchSucceeded { get; init; }

    public string? FetchError { get; init; }

    /// <summary>True when nothing needed saving (already matched) or the save itself succeeded.
    /// False whenever <see cref="FetchSucceeded"/> is false.</summary>
    public bool SaveSucceeded { get; init; }

    public int? StatusCode { get; init; }

    public string? SaveError { get; init; }

    /// <summary>Fields that existed on the fetched record and were changed.</summary>
    public List<UserFieldChange> Changes { get; init; } = new();

    /// <summary>Field names from the row that didn't exist on the fetched record at all, so were
    /// added as new properties rather than edits to existing ones - worth flagging distinctly since
    /// it usually means a mapped column name doesn't match Incident IQ's actual field name.</summary>
    public List<string> AddedFieldNames { get; init; } = new();

    public bool IsSuccess => FetchSucceeded && SaveSucceeded;
}
