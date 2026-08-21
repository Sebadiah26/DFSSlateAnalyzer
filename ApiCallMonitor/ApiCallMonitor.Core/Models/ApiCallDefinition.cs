namespace ApiCallMonitor.Core.Models;

/// <summary>One configured API call belonging to a collection: what to request, and what a
/// "success" looks like for it. Headers are kept as raw "Key: Value" lines (one per line) rather
/// than a structured dictionary so the config UI can just be a textarea - <see cref="ParseHeaders"/>
/// does the parsing at execution time.</summary>
public class ApiCallDefinition
{
    public int Id { get; set; }

    public int CollectionId { get; set; }

    public ApiCallCollection? Collection { get; set; }

    /// <summary>Position within the collection; calls run in ascending order.</summary>
    public int Order { get; set; }

    public string Name { get; set; } = string.Empty;

    public HttpCallMethod Method { get; set; } = HttpCallMethod.Get;

    public string Url { get; set; } = string.Empty;

    /// <summary>One "Header-Name: value" pair per line.</summary>
    public string? HeadersRaw { get; set; }

    /// <summary>Raw request body. Ignored for GET/HEAD.</summary>
    public string? Body { get; set; }

    public string ContentType { get; set; } = "application/json";

    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>When set, a run is only marked successful if the response status code matches this
    /// exactly. When null, any 2xx status code counts as success.</summary>
    public int? ExpectedStatusCode { get; set; }

    /// <summary>Disabled calls are skipped when a collection runs, without being removed from it.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Parses <see cref="HeadersRaw"/> into a name/value map. Blank lines and lines without
    /// a colon are ignored; a later duplicate header name overwrites an earlier one.</summary>
    public IReadOnlyDictionary<string, string> ParseHeaders()
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(HeadersRaw))
        {
            return headers;
        }

        foreach (var line in HeadersRaw.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var separatorIndex = line.IndexOf(':');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var name = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();
            if (name.Length > 0)
            {
                headers[name] = value;
            }
        }

        return headers;
    }
}
