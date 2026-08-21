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
- **Incident IQ user editor** (`/incidentiq/users`) - a separate mode for reviewing and editing
  Incident IQ users one at a time: paste in a list of user IDs, and for each one the app fetches
  the full record, shows every editable (scalar) field in a form, and only POSTs it back to
  Incident IQ when you click "Save & Next" ("Skip" moves on without changing anything). Set your
  connection (API base URL, token, site id) once via the "Configure Connection" button - it's
  shared across the whole editor, so you don't re-paste it per call. Every save is logged to
  `/incidentiq/history` with the before/after value of each field that changed, plus the status
  code Incident IQ returned.

  This deliberately doesn't hard-code Incident IQ's user schema: nested/array fields (roles,
  custom field groups, etc.) are shown read-only rather than editable, but are always carried
  through unchanged in what gets posted back. That matters because Incident IQ's update endpoint
  expects the *entire* user object on every POST - anything left out gets nulled, not left alone
  (per Incident IQ's own community docs) - so this app always fetches first and round-trips
  whatever it doesn't understand, rather than risking blanking fields it can't see.
- **Incident IQ bulk update** (`/incidentiq/bulk-update`) - the same fetch/merge/post machinery as
  the user editor, but driven by data instead of a human clicking through each record: paste a CSV
  (or tab-separated data - e.g. straight out of an SSMS results grid or Excel), pick which column is
  the Incident IQ user id, and map every other column to the Incident IQ field name it should set -
  e.g. a `UserId,JobTitle` export from a SQL query updates 1,000 users' job titles unattended. Each
  row is fetched, has just its mapped field(s) changed, and posted back - everything else on the
  record is left exactly as fetched, same as the one-at-a-time editor and for the same reason
  (Incident IQ nulls out anything missing from a POST). Rows are processed one at a time, with a
  configurable delay between them (default 250ms) so a big batch doesn't hammer Incident IQ's API,
  and results stream in live with running success/fail counts and a Cancel button. Every row - success
  or failure - is logged to the same `/incidentiq/history` as the interactive editor, tagged with a
  batch id so you can tell a bulk run's edits apart from manual ones.

  This mode posts to Incident IQ as it runs, with no draft/review step beyond the preview table and
  a one-time confirmation dialog before it starts - there's no undo. It also only runs while you stay
  on the page (navigating away cancels the batch, same as clicking Cancel) - it isn't a background
  job that survives closing the tab. For anything large or unfamiliar, try a handful of rows first.

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
| `ApiCallMonitor.Core`       | Models (`ApiCallCollection`, `ApiCallDefinition`, `CallRun`, `CallRunResult`, `UserEditLogEntry`, `IncidentIqConnectionSettings`), `IHttpCallExecutor` (sends one configured call over HTTP and reports what happened), `IPowerShellScriptGenerator` (renders a collection as a `.ps1` script), the built-in Incident IQ seed data, `ApiCallMonitor.Core.Csv` (`DelimitedTextParser`, a small dependency-free CSV/TSV parser), and `ApiCallMonitor.Core.IncidentIq` (`IIncidentIqUserClient` fetches/saves one user; `JsonRecordEditor` splits a fetched record into editable scalar fields and merges edits back without touching anything it doesn't understand; `IIncidentIqBulkFieldUpdateService` applies a fixed set of field values to one user, built on the same `JsonRecordEditor`). |
| `ApiCallMonitor.Data`       | `ApiMonitorDbContext` (EF Core + SQLite), `IRunOrchestrator` (runs every enabled call in a collection in order and persists a result row for each), `BuiltInConfigurationSeeder` (seeds the Incident IQ starter collections into a brand-new database), `IncidentIqConnectionStore` (get-or-create for the single shared Incident IQ connection), and `IBulkUserEditOrchestrator` (drives a bulk update batch row by row, with a delay between calls, logging each row as it goes). |
| `ApiCallMonitor.Blazor`     | The Blazor Server UI (MudBlazor) - collection/call configuration, the live run monitor, run history, the PowerShell script download endpoint/on-disk sync (`ScriptFileStore`), and the Incident IQ user editor + bulk update + their shared edit history. |

This is a separate solution (`ApiCallMonitor.sln`) from `DFSSlateAnalyzer.sln` at the repo root -
it isn't related to the DFS slate analyzer or staff management apps, it just lives in the same repo.

## Notes / things to add before exposing this beyond local use

- There's no authentication - anyone who can reach the site can configure and run calls, including
  whatever headers (e.g. bearer tokens) you put in them. Fine for local/internal use; add auth
  before deploying it anywhere reachable by untrusted users. The same goes for the generated
  PowerShell scripts and the on-disk `App_Data/Scripts/` folder - they contain whatever headers
  (including bearer tokens) the source calls do, in plain text.
- Response bodies are truncated to ~4KB when stored, to keep the database small.
- The built-in Incident IQ calls, and the user editor's `GET`/`POST` to `{ApiBaseUrl}/users/{id}`,
  are a starting point, not a guarantee: the paths/headers/update semantics were pulled from
  Incident IQ's public docs and community posts, not an official, versioned spec, so double-check
  them against Administration > Developer Tools in your own Incident IQ site - and try the user
  editor against a test/non-critical account first.
- The user editor posts directly to Incident IQ the moment you click "Save & Next" - there's no
  draft/approval step. Double-check the fields (and the "was: ..." helper text under anything
  you've changed) before saving.
