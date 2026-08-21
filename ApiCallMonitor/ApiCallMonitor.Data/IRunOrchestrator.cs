using ApiCallMonitor.Core.Models;

namespace ApiCallMonitor.Data;

/// <summary>Drives one run of a collection: loads its enabled calls, executes them in order via
/// IHttpCallExecutor, and persists a CallRun/CallRunResult row for each. Split into two steps -
/// <see cref="CreateRunAsync"/> then <see cref="ExecuteRunAsync"/> - so a caller (the "Run Now"
/// button) can get a run id back immediately, navigate the user to its monitor page, and only then
/// kick off the actual (slower) execution in the background.</summary>
public interface IRunOrchestrator
{
    /// <summary>Creates the CallRun row (Status = Running) and returns its id. Does not execute any
    /// calls yet.</summary>
    Task<int> CreateRunAsync(int collectionId, CancellationToken cancellationToken = default);

    /// <summary>Executes every enabled call in the run's collection, in order, persisting a result
    /// row after each one and reporting it to <paramref name="progress"/> as it happens. Updates and
    /// returns the run's final status. Safe to call from a background task outside of any HTTP
    /// request/circuit scope.</summary>
    Task<RunStatus> ExecuteRunAsync(int runId, IProgress<CallRunResult>? progress = null, CancellationToken cancellationToken = default);
}
