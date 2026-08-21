using ApiCallMonitor.Core.Models;

namespace ApiCallMonitor.Core.Export;

/// <summary>Renders a saved collection as a standalone PowerShell script that replays the same
/// calls the app itself would run - so a collection can be scheduled (Task Scheduler, cron via
/// pwsh, a CI pipeline) or handed to someone else without this app running at all.</summary>
public interface IPowerShellScriptGenerator
{
    /// <summary>Builds the .ps1 source for <paramref name="collection"/>. Only enabled calls are
    /// included, in <see cref="ApiCallDefinition.Order"/> order - disabled calls are skipped, same
    /// as a normal run. <paramref name="collection"/>.Calls must already be loaded.</summary>
    string Generate(ApiCallCollection collection);
}
