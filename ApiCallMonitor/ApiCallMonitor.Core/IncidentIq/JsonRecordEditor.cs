using System.Text.Json;
using System.Text.Json.Nodes;

namespace ApiCallMonitor.Core.IncidentIq;

/// <summary>Splits a fetched JSON record into editable top-level scalar fields, and merges edits
/// back into a full copy of the original record.
///
/// Incident IQ's own docs/community guidance say updating a user is a POST of the *entire* user
/// object back to its endpoint - any field left out of that payload gets nulled out, not left
/// alone. Rather than hard-coding Incident IQ's user schema (which isn't fully documented
/// publicly, and could vary by site/version anyway), this only ever edits scalar properties the
/// operator explicitly changed and carries every other property - nested objects/arrays included -
/// through untouched, so a save can never silently blank out something this app doesn't know
/// about.</summary>
public static class JsonRecordEditor
{
    public static List<EditableJsonField> ExtractEditableFields(JsonObject record)
    {
        var fields = new List<EditableJsonField>();

        foreach (var (name, node) in record)
        {
            if (node is null)
            {
                fields.Add(new EditableJsonField { Name = name, OriginalKind = JsonValueKind.Null, OriginalValue = null, EditedValue = null });
                continue;
            }

            if (node is JsonValue value)
            {
                var kind = value.GetValueKind();
                var stringValue = FormatScalar(value, kind);
                fields.Add(new EditableJsonField { Name = name, OriginalKind = kind, OriginalValue = stringValue, EditedValue = stringValue });
            }

            // JsonObject/JsonArray properties are intentionally left out - see the class doc comment.
        }

        return fields;
    }

    /// <summary>Clones <paramref name="original"/>, applies every changed field from
    /// <paramref name="fields"/> onto the clone (converted back to the field's original JSON type
    /// where possible), and returns the merged record plus the subset of fields that actually
    /// changed.</summary>
    public static (JsonObject Merged, List<EditableJsonField> Changes) ApplyEdits(JsonObject original, IReadOnlyList<EditableJsonField> fields)
    {
        var merged = original.DeepClone().AsObject();
        var changes = new List<EditableJsonField>();

        foreach (var field in fields)
        {
            if (!field.Changed)
            {
                continue;
            }

            merged[field.Name] = ParseAsOriginalKind(field.EditedValue, field.OriginalKind);
            changes.Add(field);
        }

        return (merged, changes);
    }

    private static string? FormatScalar(JsonValue value, JsonValueKind kind) => kind switch
    {
        JsonValueKind.String => value.GetValue<string>(),
        JsonValueKind.True or JsonValueKind.False => value.GetValue<bool>().ToString(),
        _ => value.ToJsonString(),
    };

    private static JsonNode? ParseAsOriginalKind(string? editedValue, JsonValueKind originalKind)
    {
        if (editedValue is null)
        {
            return null;
        }

        return originalKind switch
        {
            JsonValueKind.True or JsonValueKind.False =>
                bool.TryParse(editedValue, out var b) ? JsonValue.Create(b) : JsonValue.Create(editedValue),
            JsonValueKind.Number =>
                long.TryParse(editedValue, out var l) ? JsonValue.Create(l)
                    : double.TryParse(editedValue, out var d) ? JsonValue.Create(d)
                    : JsonValue.Create(editedValue),
            _ => JsonValue.Create(editedValue),
        };
    }
}
