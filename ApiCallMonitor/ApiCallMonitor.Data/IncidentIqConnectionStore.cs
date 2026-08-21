using ApiCallMonitor.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCallMonitor.Data;

/// <summary>Get-or-create/upsert helper for the single <see cref="IncidentIqConnectionSettings"/>
/// row. Kept out of the Blazor pages so both the user editor page and the connection-editing dialog
/// share the exact same "row might not exist yet" handling.</summary>
public static class IncidentIqConnectionStore
{
    public static async Task<IncidentIqConnectionSettings> GetAsync(ApiMonitorDbContext db, CancellationToken cancellationToken = default)
    {
        var settings = await db.IncidentIqConnectionSettings.FindAsync(new object[] { IncidentIqConnectionSettings.SingletonId }, cancellationToken);
        return settings ?? new IncidentIqConnectionSettings();
    }

    public static async Task SaveAsync(ApiMonitorDbContext db, IncidentIqConnectionSettings updated, CancellationToken cancellationToken = default)
    {
        var existing = await db.IncidentIqConnectionSettings.FindAsync(new object[] { IncidentIqConnectionSettings.SingletonId }, cancellationToken);

        if (existing is null)
        {
            db.IncidentIqConnectionSettings.Add(new IncidentIqConnectionSettings
            {
                ApiBaseUrl = updated.ApiBaseUrl,
                ApiToken = updated.ApiToken,
                SiteId = updated.SiteId,
            });
        }
        else
        {
            existing.ApiBaseUrl = updated.ApiBaseUrl;
            existing.ApiToken = updated.ApiToken;
            existing.SiteId = updated.SiteId;
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
