namespace ApiCallMonitor.Core.Models;

/// <summary>Lifecycle of a single run of a collection. Stored as a string (see
/// ApiMonitorDbContext) so it stays readable in the database directly.</summary>
public enum RunStatus
{
    Running,
    Completed,
    CompletedWithErrors,
    Cancelled,
}
