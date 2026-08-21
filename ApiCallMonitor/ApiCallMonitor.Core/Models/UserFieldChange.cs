namespace ApiCallMonitor.Core.Models;

/// <summary>One field's before/after value, as recorded in a <see cref="UserEditLogEntry"/>'s
/// ChangedFieldsJson.</summary>
public record UserFieldChange(string Field, string? OldValue, string? NewValue);
