using System.Text.Json;
using ApiCallMonitor.Core.IncidentIq;
using ApiCallMonitor.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiCallMonitor.Data;

/// <summary>Default <see cref="IBulkUserEditOrchestrator"/>. Processes rows strictly one at a time
/// (not in parallel) with an optional delay between calls, so a large batch (hundreds/thousands of
/// users) doesn't hammer Incident IQ's API or trip a rate limit. Never aborts the batch on one row's
/// failure - keeps going and tallies results, same philosophy as the generic collection runner.</summary>
public class BulkUserEditOrchestrator : IBulkUserEditOrchestrator
{
    private readonly IIncidentIqBulkFieldUpdateService _updateService;
    private readonly IDbContextFactory<ApiMonitorDbContext> _dbContextFactory;
    private readonly ILogger<BulkUserEditOrchestrator> _logger;

    public BulkUserEditOrchestrator(
        IIncidentIqBulkFieldUpdateService updateService,
        IDbContextFactory<ApiMonitorDbContext> dbContextFactory,
        ILogger<BulkUserEditOrchestrator> logger)
    {
        _updateService = updateService;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task RunBatchAsync(
        IncidentIqConnectionSettings settings,
        string batchId,
        IReadOnlyList<BulkUpdateRow> rows,
        TimeSpan delayBetweenCalls,
        IProgress<BulkFieldUpdateRowResult>? progress = null,
        CancellationToken cancellationToken = default)
    {
        for (var i = 0; i < rows.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var row = rows[i];
            BulkFieldUpdateRowResult result;
            try
            {
                result = await _updateService.ApplyFieldUpdatesAsync(settings, row.UserId, row.FieldValues, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Bulk update failed unexpectedly for user {UserId}.", row.UserId);
                result = new BulkFieldUpdateRowResult { UserId = row.UserId, FetchSucceeded = false, FetchError = ex.Message };
            }

            await LogAsync(batchId, result, cancellationToken);
            progress?.Report(result);

            var isLastRow = i == rows.Count - 1;
            if (!isLastRow && delayBetweenCalls > TimeSpan.Zero)
            {
                await Task.Delay(delayBetweenCalls, cancellationToken);
            }
        }
    }

    private async Task LogAsync(string batchId, BulkFieldUpdateRowResult result, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        db.UserEditLog.Add(new UserEditLogEntry
        {
            IncidentIqUserId = result.UserId,
            UserDisplayName = result.DisplayName,
            BatchId = batchId,
            ChangedFieldsJson = JsonSerializer.Serialize(result.Changes),
            StatusCode = result.StatusCode,
            IsSuccess = result.IsSuccess,
            ErrorMessage = result.FetchError ?? result.SaveError,
        });
        await db.SaveChangesAsync(cancellationToken);
    }
}
