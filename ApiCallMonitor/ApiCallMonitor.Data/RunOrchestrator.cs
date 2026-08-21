using ApiCallMonitor.Core.Execution;
using ApiCallMonitor.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiCallMonitor.Data;

/// <summary>Default <see cref="IRunOrchestrator"/>. Uses an <see cref="IDbContextFactory{TContext}"/>
/// rather than an injected DbContext so it can safely run from a background task that outlives the
/// request/circuit that started it - see the "Run Now" flow in the Blazor project.</summary>
public class RunOrchestrator : IRunOrchestrator
{
    private readonly IDbContextFactory<ApiMonitorDbContext> _dbContextFactory;
    private readonly IHttpCallExecutor _executor;
    private readonly ILogger<RunOrchestrator> _logger;

    public RunOrchestrator(IDbContextFactory<ApiMonitorDbContext> dbContextFactory, IHttpCallExecutor executor, ILogger<RunOrchestrator> logger)
    {
        _dbContextFactory = dbContextFactory;
        _executor = executor;
        _logger = logger;
    }

    public async Task<int> CreateRunAsync(int collectionId, CancellationToken cancellationToken = default)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var collectionExists = await db.Collections.AnyAsync(c => c.Id == collectionId, cancellationToken);
        if (!collectionExists)
        {
            throw new InvalidOperationException($"Collection {collectionId} does not exist.");
        }

        var run = new CallRun
        {
            CollectionId = collectionId,
            StartedAtUtc = DateTime.UtcNow,
            Status = RunStatus.Running,
        };

        db.Runs.Add(run);
        await db.SaveChangesAsync(cancellationToken);

        return run.Id;
    }

    public async Task<RunStatus> ExecuteRunAsync(int runId, IProgress<CallRunResult>? progress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var run = await db.Runs.FirstOrDefaultAsync(r => r.Id == runId, cancellationToken)
                ?? throw new InvalidOperationException($"Run {runId} does not exist.");

            var calls = await db.Calls
                .Where(c => c.CollectionId == run.CollectionId && c.Enabled)
                .OrderBy(c => c.Order)
                .ToListAsync(cancellationToken);

            var hadFailure = false;

            try
            {
                foreach (var call in calls)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var result = await _executor.ExecuteAsync(call, cancellationToken);
                    result.CallRunId = run.Id;

                    if (!result.IsSuccess)
                    {
                        hadFailure = true;
                    }

                    db.RunResults.Add(result);
                    await db.SaveChangesAsync(cancellationToken);

                    progress?.Report(result);
                }
            }
            catch (OperationCanceledException)
            {
                run.CompletedAtUtc = DateTime.UtcNow;
                run.Status = RunStatus.Cancelled;
                await db.SaveChangesAsync(CancellationToken.None);
                return run.Status;
            }

            run.CompletedAtUtc = DateTime.UtcNow;
            run.Status = hadFailure ? RunStatus.CompletedWithErrors : RunStatus.Completed;
            await db.SaveChangesAsync(cancellationToken);

            return run.Status;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Belt-and-braces: an exception here means something outside a single call's own
            // execution went wrong (e.g. the database itself became unreachable mid-run). Without
            // this, the run row would stay stuck at Status = Running forever, and any monitor page
            // watching it would spin indefinitely. Best-effort - if even this can't reach the
            // database, there's nothing left to record it with.
            _logger.LogError(ex, "Run {RunId} failed unexpectedly and did not complete normally.", runId);

            try
            {
                await using var db = await _dbContextFactory.CreateDbContextAsync(CancellationToken.None);
                var run = await db.Runs.FirstOrDefaultAsync(r => r.Id == runId, CancellationToken.None);
                if (run is not null && run.Status == RunStatus.Running)
                {
                    run.CompletedAtUtc = DateTime.UtcNow;
                    run.Status = RunStatus.CompletedWithErrors;
                    await db.SaveChangesAsync(CancellationToken.None);
                }
            }
            catch (Exception markFailedEx)
            {
                _logger.LogError(markFailedEx, "Also failed to mark run {RunId} as failed after the original error.", runId);
            }

            return RunStatus.CompletedWithErrors;
        }
    }
}
