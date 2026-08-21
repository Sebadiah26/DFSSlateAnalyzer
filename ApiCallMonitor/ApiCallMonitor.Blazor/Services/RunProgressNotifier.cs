using ApiCallMonitor.Core.Models;

namespace ApiCallMonitor.Blazor.Services;

/// <summary>Singleton in-process pub/sub that fans out live run progress to whichever monitor page
/// (RunDetails.razor) happens to be watching a given run id. Kept decoupled from IRunOrchestrator
/// itself (which only knows about IProgress&lt;CallRunResult&gt;) so the Data project stays free of
/// any Blazor-specific concerns - the "Run Now" button wires the two together, see
/// Collections/Details.razor.</summary>
public class RunProgressNotifier
{
    public event Action<int, CallRunResult>? ResultReported;

    public event Action<int, RunStatus>? RunFinished;

    public void ReportResult(int runId, CallRunResult result) => ResultReported?.Invoke(runId, result);

    public void ReportRunFinished(int runId, RunStatus status) => RunFinished?.Invoke(runId, status);
}
