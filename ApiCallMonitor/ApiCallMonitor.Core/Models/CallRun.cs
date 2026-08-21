namespace ApiCallMonitor.Core.Models;

/// <summary>One execution of a collection's calls, from "Run" clicked to the last call finishing
/// (or the run being cancelled). <see cref="Results"/> fills in one row at a time as each call
/// completes, which is what the monitor page watches to show live progress.</summary>
public class CallRun
{
    public int Id { get; set; }

    public int CollectionId { get; set; }

    public ApiCallCollection? Collection { get; set; }

    public DateTime StartedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public RunStatus Status { get; set; } = RunStatus.Running;

    public List<CallRunResult> Results { get; set; } = new();
}
