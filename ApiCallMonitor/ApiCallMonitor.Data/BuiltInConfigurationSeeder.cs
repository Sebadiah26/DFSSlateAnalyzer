using ApiCallMonitor.Core.Seed;
using Microsoft.EntityFrameworkCore;

namespace ApiCallMonitor.Data;

/// <summary>Seeds the built-in starter collections (see IncidentIqBuiltInConfigurations) into a
/// brand-new database. Only runs when the Collections table is completely empty, so it never
/// clobbers anything a user has since created, edited, or deleted - including deleting the
/// built-ins themselves.</summary>
public static class BuiltInConfigurationSeeder
{
    public static async Task EnsureSeededAsync(ApiMonitorDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Collections.AnyAsync(cancellationToken))
        {
            return;
        }

        db.Collections.AddRange(IncidentIqBuiltInConfigurations.BuildAll());
        await db.SaveChangesAsync(cancellationToken);
    }
}
