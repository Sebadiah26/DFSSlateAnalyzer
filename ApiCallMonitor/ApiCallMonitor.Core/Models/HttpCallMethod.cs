namespace ApiCallMonitor.Core.Models;

/// <summary>HTTP verbs a configured call can use. Stored as a string in the database (see
/// ApiMonitorDbContext) so the persisted value stays readable and stable across enum reordering.</summary>
public enum HttpCallMethod
{
    Get,
    Post,
    Put,
    Patch,
    Delete,
    Head,
    Options,
}
