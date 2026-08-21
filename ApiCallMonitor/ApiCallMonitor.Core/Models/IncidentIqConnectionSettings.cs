namespace ApiCallMonitor.Core.Models;

/// <summary>The single shared Incident IQ connection used by the user editor (see
/// ApiCallMonitor.Core.IncidentIq and Pages/IncidentIq in the Blazor project). Kept as one row
/// (Id is always <see cref="SingletonId"/>) rather than a list of named connections, since this
/// covers one district's Incident IQ site at a time.</summary>
public class IncidentIqConnectionSettings
{
    public const int SingletonId = 1;

    public int Id { get; set; } = SingletonId;

    /// <summary>Full API base, e.g. "https://yoursite.incidentiq.com/api/v1.0" - the same shape
    /// Incident IQ's own docs and community examples use, so it can be pasted straight from there.</summary>
    public string ApiBaseUrl { get; set; } = string.Empty;

    public string ApiToken { get; set; } = string.Empty;

    public string SiteId { get; set; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ApiBaseUrl) && !string.IsNullOrWhiteSpace(ApiToken) && !string.IsNullOrWhiteSpace(SiteId);
}
