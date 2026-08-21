using ApiCallMonitor.Core.Models;

namespace ApiCallMonitor.Core.Execution;

/// <summary>Executes a single configured call over HTTP and reports what happened. Knows nothing
/// about collections, runs, or persistence - that orchestration lives in ApiCallMonitor.Data's
/// IRunOrchestrator, which calls this once per enabled call in a collection.</summary>
public interface IHttpCallExecutor
{
    Task<CallRunResult> ExecuteAsync(ApiCallDefinition call, CancellationToken cancellationToken = default);
}
