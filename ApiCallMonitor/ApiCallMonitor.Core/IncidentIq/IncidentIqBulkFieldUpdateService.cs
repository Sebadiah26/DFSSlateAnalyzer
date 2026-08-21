using System.Text.Json.Nodes;
using ApiCallMonitor.Core.Models;

namespace ApiCallMonitor.Core.IncidentIq;

/// <summary>Default <see cref="IIncidentIqBulkFieldUpdateService"/>: fetch the user, set the given
/// field values onto its <see cref="EditableJsonField"/>s (same as an operator typing into the
/// interactive editor would), and post the merged record back if anything actually changed - never
/// posting an unnecessary identical copy.</summary>
public class IncidentIqBulkFieldUpdateService : IIncidentIqBulkFieldUpdateService
{
    private static readonly string[] DisplayNameCandidates = { "Name", "FullName", "DisplayName", "Email", "Username", "UserName" };

    private readonly IIncidentIqUserClient _userClient;

    public IncidentIqBulkFieldUpdateService(IIncidentIqUserClient userClient) => _userClient = userClient;

    public async Task<BulkFieldUpdateRowResult> ApplyFieldUpdatesAsync(
        IncidentIqConnectionSettings settings,
        string userId,
        IReadOnlyDictionary<string, string> fieldValues,
        CancellationToken cancellationToken = default)
    {
        var fetch = await _userClient.GetUserAsync(settings, userId, cancellationToken);
        if (!fetch.Success || fetch.User is null)
        {
            return new BulkFieldUpdateRowResult
            {
                UserId = userId,
                FetchSucceeded = false,
                FetchError = fetch.ErrorMessage ?? $"Fetch failed ({fetch.StatusCode}).",
            };
        }

        var fields = JsonRecordEditor.ExtractEditableFields(fetch.User);
        var addedFieldNames = new List<string>();

        foreach (var (name, value) in fieldValues)
        {
            var match = fields.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
            if (match is not null)
            {
                match.EditedValue = value;
            }
            else
            {
                addedFieldNames.Add(name);
            }
        }

        var displayName = GetDisplayName(fields);
        var (merged, changes) = JsonRecordEditor.ApplyEdits(fetch.User, fields);

        foreach (var name in addedFieldNames)
        {
            merged[name] = JsonValue.Create(fieldValues[name]);
        }

        if (changes.Count == 0 && addedFieldNames.Count == 0)
        {
            // Nothing to do - don't spend an API call re-posting an identical record.
            return new BulkFieldUpdateRowResult { UserId = userId, DisplayName = displayName, FetchSucceeded = true, SaveSucceeded = true };
        }

        var saveResult = await _userClient.SaveUserAsync(settings, userId, merged, cancellationToken);

        return new BulkFieldUpdateRowResult
        {
            UserId = userId,
            DisplayName = displayName,
            FetchSucceeded = true,
            SaveSucceeded = saveResult.Success,
            StatusCode = saveResult.StatusCode,
            SaveError = saveResult.ErrorMessage,
            Changes = changes.Select(f => new UserFieldChange(f.Name, f.OriginalValue, f.EditedValue)).ToList(),
            AddedFieldNames = addedFieldNames,
        };
    }

    private static string? GetDisplayName(List<EditableJsonField> fields)
    {
        foreach (var candidate in DisplayNameCandidates)
        {
            var match = fields.FirstOrDefault(f => string.Equals(f.Name, candidate, StringComparison.OrdinalIgnoreCase));
            if (match is not null && !string.IsNullOrWhiteSpace(match.OriginalValue))
            {
                return match.OriginalValue;
            }
        }

        return null;
    }
}
