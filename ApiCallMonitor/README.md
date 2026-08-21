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
| `ApiCallMonitor.Core`       | Models (`ApiCallCollection`, `ApiCallDefinition`, `CallRun`, `CallRunResult`) and `IHttpCallExecutor`, which sends one configured call over HTTP and reports what happened. |
| `ApiCallMonitor.Data`       | `ApiMonitorDbContext` (EF Core + SQLite) and `IRunOrchestrator`, which runs every enabled call in a collection in order and persists a result row for each. |
| `ApiCallMonitor.Blazor`     | The Blazor Server UI (MudBlazor) - collection/call configuration, the live run monitor, and run history. |

This is a separate solution (`ApiCallMonitor.sln`) from `DFSSlateAnalyzer.sln` at the repo root -
it isn't related to the DFS slate analyzer or staff management apps, it just lives in the same repo.

## Notes / things to add before exposing this beyond local use

- There's no authentication - anyone who can reach the site can configure and run calls, including
  whatever headers (e.g. bearer tokens) you put in them. Fine for local/internal use; add auth
  before deploying it anywhere reachable by untrusted users.
- Response bodies are truncated to ~4KB when stored, to keep the database small.
