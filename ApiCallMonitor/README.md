# API Call Monitor

A small Blazor Server tool for configuring a set of API calls, running them, and watching the
results as they come in.

- **Collections** - a named, ordered set of calls (e.g. "Nightly health checks").
- **Calls** - each call configures a method, URL, headers, body, timeout, and (optionally) an
  expected status code that defines "success" for it.
- **Run Now** - executes every enabled call in a collection, in order, and takes you straight to a
  live monitor page that fills in each call's result (status code, duration, response body/error)
  as it finishes.
- **History** - every run is kept, so past results stay browsable after the fact.
- **PowerShell export** - every saved collection also gets a runnable `.ps1` script, kept in sync
  automatically: it's rewritten to `ApiCallMonitor.Blazor/App_Data/Scripts/collection-{id}.ps1`
  every time you save the collection or its calls, and there's a "PowerShell Script" button/download
  icon (on the collection page and the collections list) that generates a fresh copy on demand. The
  script replays the collection's enabled calls with `Invoke-WebRequest`, prints a pass/fail summary,
  and exits 0/1 - so it can run unattended from Task Scheduler, cron (via `pwsh`), or a CI pipeline
  without this app running at all. Works on Windows PowerShell 5.1+ and PowerShell 7+.
- **Built-in Incident IQ collections** - a fresh database is seeded with two starter collections
  ("Incident IQ - Reference Data" and "Incident IQ - Assets & Users") covering common Incident IQ
  read-only endpoints, marked with a "Built-in" chip. Fill in your site/token/site-id placeholders
  (see each collection's description) before running them - see the caveat below.

## Running it

```
cd ApiCallMonitor
dotnet run --project ApiCallMonitor.Blazor
```

Then open the URL `dotnet run` prints (defaults to `http://localhost:5180`).

No external database is required - it stores everything in a local SQLite file at
`ApiCallMonitor.Blazor/App_Data/apicallmonitor.db`, created automatically on first run. To point it
at a different location instead, set `ConnectionStrings:ApiCallMonitorDb` (e.g. in
`appsettings.Development.json` or user secrets).

## Project layout

| Project                    | Contents                                                                 |
|-----------------------------|---------------------------------------------------------------------------|
| `ApiCallMonitor.Core`       | Models (`ApiCallCollection`, `ApiCallDefinition`, `CallRun`, `CallRunResult`), `IHttpCallExecutor` (sends one configured call over HTTP and reports what happened), `IPowerShellScriptGenerator` (renders a collection as a `.ps1` script), and the built-in Incident IQ seed data. |
| `ApiCallMonitor.Data`       | `ApiMonitorDbContext` (EF Core + SQLite), `IRunOrchestrator` (runs every enabled call in a collection in order and persists a result row for each), and `BuiltInConfigurationSeeder` (seeds the Incident IQ starter collections into a brand-new database). |
| `ApiCallMonitor.Blazor`     | The Blazor Server UI (MudBlazor) - collection/call configuration, the live run monitor, run history, and the PowerShell script download endpoint/on-disk sync (`ScriptFileStore`). |

This is a separate solution (`ApiCallMonitor.sln`) from `DFSSlateAnalyzer.sln` at the repo root -
it isn't related to the DFS slate analyzer or staff management apps, it just lives in the same repo.

## Notes / things to add before exposing this beyond local use

- There's no authentication - anyone who can reach the site can configure and run calls, including
  whatever headers (e.g. bearer tokens) you put in them. Fine for local/internal use; add auth
  before deploying it anywhere reachable by untrusted users. The same goes for the generated
  PowerShell scripts and the on-disk `App_Data/Scripts/` folder - they contain whatever headers
  (including bearer tokens) the source calls do, in plain text.
- Response bodies are truncated to ~4KB when stored, to keep the database small.
- The built-in Incident IQ calls are a starting point, not a guarantee: the paths/headers were
  pulled from Incident IQ's public docs and community posts, not an official, versioned spec, so
  double-check them against Administration > Developer Tools in your own Incident IQ site.
