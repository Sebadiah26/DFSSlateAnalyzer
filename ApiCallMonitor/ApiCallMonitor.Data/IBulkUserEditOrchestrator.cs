using ApiCallMonitor.Core.IncidentIq;
using ApiCallMonitor.Core.Models;

namespace ApiCallMonitor.Data;

/// <summary>Drives a bulk update: applies field values to many users in order, one Incident IQ
/// fetch+save round trip at a time, persisting a <see cref="UserEditLogEntry"/> per row as it goes
/// and reporting each row's outcome so a page can show live progress.</summary>
public interface IBulkUserEditOrchestrator
{
    Task RunBatchAsync(
        IncidentIqConnectionSettings settings,
        string batchId,
        IReadOnlyList<BulkUpdateRow> rows,
        TimeSpan delayBetweenCalls,
        IProgress<BulkFieldUpdateRowResult>? progress = null,
        CancellationToken cancellationToken = default);
}
