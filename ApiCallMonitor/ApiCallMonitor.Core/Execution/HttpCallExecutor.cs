using System.Diagnostics;
using System.Text;
using ApiCallMonitor.Core.Models;
using Microsoft.Extensions.Logging;

namespace ApiCallMonitor.Core.Execution;

/// <summary>Default <see cref="IHttpCallExecutor"/>: builds an <see cref="HttpRequestMessage"/>
/// from an <see cref="ApiCallDefinition"/>, sends it, and turns the outcome - success, an
/// unexpected status code, a timeout, or a transport-level failure - into a <see cref="CallRunResult"/>.
/// Never throws: every failure mode is captured on the result so a whole collection run can keep
/// going past one bad call.</summary>
public class HttpCallExecutor : IHttpCallExecutor
{
    /// <summary>Name registered with IHttpClientFactory for the shared client this executor uses.
    /// Per-call timeouts are enforced independently via a linked CancellationTokenSource (see
    /// ExecuteAsync), so the client itself just needs a generous outer ceiling.</summary>
    public const string HttpClientName = "ApiCallMonitor";

    private const int MaxResponseSnippetLength = 4000;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HttpCallExecutor> _logger;

    public HttpCallExecutor(IHttpClientFactory httpClientFactory, ILogger<HttpCallExecutor> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<CallRunResult> ExecuteAsync(ApiCallDefinition call, CancellationToken cancellationToken = default)
    {
        var result = new CallRunResult
        {
            ApiCallDefinitionId = call.Id,
            Order = call.Order,
            Name = call.Name,
            Method = call.Method.ToString().ToUpperInvariant(),
            Url = call.Url,
            ExecutedAtUtc = DateTime.UtcNow,
        };

        var stopwatch = Stopwatch.StartNew();

        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(Math.Max(1, call.TimeoutSeconds)));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            using var request = BuildRequest(call);
            var client = _httpClientFactory.CreateClient(HttpClientName);

            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, linkedCts.Token);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            stopwatch.Stop();
            result.DurationMs = stopwatch.ElapsedMilliseconds;
            result.StatusCode = (int)response.StatusCode;
            result.ResponseSnippet = Truncate(responseBody, MaxResponseSnippetLength);
            result.IsSuccess = call.ExpectedStatusCode.HasValue
                ? (int)response.StatusCode == call.ExpectedStatusCode.Value
                : response.IsSuccessStatusCode;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // The linked token fired but the caller's own token didn't: this call's own timeout
            // elapsed, not a cancellation of the whole run.
            stopwatch.Stop();
            result.DurationMs = stopwatch.ElapsedMilliseconds;
            result.IsSuccess = false;
            result.ErrorMessage = $"Timed out after {call.TimeoutSeconds}s.";
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            stopwatch.Stop();
            result.DurationMs = stopwatch.ElapsedMilliseconds;
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            _logger.LogWarning(ex, "API call '{CallName}' ({Method} {Url}) failed to execute.", call.Name, result.Method, call.Url);
        }

        return result;
    }

    private static HttpRequestMessage BuildRequest(ApiCallDefinition call)
    {
        var method = call.Method switch
        {
            HttpCallMethod.Get => HttpMethod.Get,
            HttpCallMethod.Post => HttpMethod.Post,
            HttpCallMethod.Put => HttpMethod.Put,
            HttpCallMethod.Patch => HttpMethod.Patch,
            HttpCallMethod.Delete => HttpMethod.Delete,
            HttpCallMethod.Head => HttpMethod.Head,
            HttpCallMethod.Options => HttpMethod.Options,
            _ => HttpMethod.Get,
        };

        var request = new HttpRequestMessage(method, call.Url);

        if (!string.IsNullOrEmpty(call.Body) && method != HttpMethod.Get && method != HttpMethod.Head)
        {
            var contentType = string.IsNullOrWhiteSpace(call.ContentType) ? "application/json" : call.ContentType;
            request.Content = new StringContent(call.Body, Encoding.UTF8, contentType);
        }

        foreach (var (name, value) in call.ParseHeaders())
        {
            // Content-* headers belong on request.Content (set above via StringContent's
            // constructor); TryAddWithoutValidation on the request itself would silently no-op them.
            if (name.StartsWith("Content-", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            request.Headers.TryAddWithoutValidation(name, value);
        }

        return request;
    }

    private static string? Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
        {
            return value;
        }

        return string.Concat(value.AsSpan(0, maxLength), $"… (truncated, {value.Length} chars total)");
    }
}
