using ApiCallMonitor.Core.Models;

namespace ApiCallMonitor.Core.Seed;

/// <summary>Starter <see cref="ApiCallCollection"/>s for Incident IQ (the K-12 IT asset/ticketing
/// platform), seeded once into a brand-new database - see BuiltInConfigurationSeeder in
/// ApiCallMonitor.Data. Endpoint paths and the required header names come from Incident IQ's own
/// public API docs (https://apihub.incidentiq.com) and community posts as of when this was
/// written. Incident IQ can change or extend its API without notice and this only covers a
/// handful of read-only endpoints, so treat these as a starting point - verify against
/// Administration &gt; Developer Tools in your own Incident IQ site, not as a guarantee they're
/// still accurate.</summary>
public static class IncidentIqBuiltInConfigurations
{
    private const string BaseUrl = "https://REPLACE_WITH_YOUR_SITE.incidentiq.com/api/v1.0";

    // Every Incident IQ API request needs these three headers (confirmed in their own docs) - see
    // Administration > Developer Tools in your Incident IQ site for your API token and site id.
    private const string CommonHeaders =
        "Authorization: Bearer REPLACE_WITH_YOUR_API_TOKEN\n" +
        "SiteId: REPLACE_WITH_YOUR_SITE_ID\n" +
        "Client: ApiClient";

    private const string ReplaceHeadersNote =
        "Replace REPLACE_WITH_YOUR_SITE in each call's URL and REPLACE_WITH_YOUR_API_TOKEN / " +
        "REPLACE_WITH_YOUR_SITE_ID in its headers before running - find your API token and site id " +
        "under Administration > Developer Tools in your Incident IQ site.";

    public static IReadOnlyList<ApiCallCollection> BuildAll() => new[]
    {
        BuildReferenceDataCollection(),
        BuildAssetsAndUsersCollection(),
    };

    private static ApiCallCollection BuildReferenceDataCollection() => new()
    {
        Name = "Incident IQ - Reference Data",
        Description = $"Built-in starter calls for Incident IQ's read-only lookup endpoints. {ReplaceHeadersNote}",
        IsBuiltIn = true,
        Calls = new List<ApiCallDefinition>
        {
            NewCall(0, "Get Ticket Statuses", $"{BaseUrl}/tickets/statuses"),
            NewCall(1, "Get Categories", $"{BaseUrl}/categories?$s=1000"),
            NewCall(2, "Get All Locations", $"{BaseUrl}/locations/all"),
            NewCall(3, "Get Manufacturers", $"{BaseUrl}/manufacturers"),
        },
    };

    private static ApiCallCollection BuildAssetsAndUsersCollection() => new()
    {
        Name = "Incident IQ - Assets & Users",
        Description = $"Built-in starter calls for browsing Incident IQ assets and users. {ReplaceHeadersNote} " +
            "The 'by ID' calls also need a real GUID in place of REPLACE_WITH_ASSET_ID / REPLACE_WITH_USER_ID, " +
            "so they start out disabled - fill in an id (e.g. from one of the list calls' results) and enable them.",
        IsBuiltIn = true,
        Calls = new List<ApiCallDefinition>
        {
            NewCall(0, "List Assets (paged)", $"{BaseUrl}/assets/?$s=50&$o=AssetTag%20ASC"),
            NewCall(1, "Get Asset by ID", $"{BaseUrl}/assets/REPLACE_WITH_ASSET_ID", enabled: false),
            NewCall(2, "List Users (paged)", $"{BaseUrl}/users/?$s=50&$o=FullName%20ASC"),
            NewCall(3, "Get User by ID", $"{BaseUrl}/users/REPLACE_WITH_USER_ID", enabled: false),
        },
    };

    private static ApiCallDefinition NewCall(int order, string name, string url, bool enabled = true) => new()
    {
        Order = order,
        Name = name,
        Method = HttpCallMethod.Get,
        Url = url,
        HeadersRaw = CommonHeaders,
        TimeoutSeconds = 30,
        ExpectedStatusCode = 200,
        Enabled = enabled,
    };
}
