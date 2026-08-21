using System.Text.Json;

namespace ApiCallMonitor.Core.IncidentIq;

/// <summary>One top-level scalar (string/number/bool/null) property of a fetched Incident IQ
/// record, exposed for editing - see <see cref="JsonRecordEditor"/> for why only scalars are
/// editable. <see cref="EditedValue"/> starts out equal to <see cref="OriginalValue"/> and is what
/// the editor UI binds to.</summary>
public class EditableJsonField
{
    public required string Name { get; init; }

    public required JsonValueKind OriginalKind { get; init; }

    public required string? OriginalValue { get; init; }

    public string? EditedValue { get; set; }

    public bool Changed => !string.Equals(OriginalValue, EditedValue, StringComparison.Ordinal);
}
