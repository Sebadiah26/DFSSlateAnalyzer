using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ApiCallMonitor.Core.Execution;
using ApiCallMonitor.Core.Models;
using Microsoft.Extensions.Logging;

namespace ApiCallMonitor.Core.IncidentIq;

/// <summary>Default <see cref="IIncidentIqUserClient"/>. GET to fetch, POST to save - both to
/// <c>{ApiBaseUrl}/users/{userId}</c> - with the Authorization/SiteId/Client headers Incident IQ's
/// API requires on every request. Uses the same shared named HttpClient as the rest of the app
/// (see <see cref="HttpCallExecutor.HttpClientName"/>).</summary>
public class IncidentIqUserClient : IIncidentIqUserClient
{
    private const int MaxResponseSnippetLength = 4000;
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(30);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<IncidentIqUserClient> _logger;

    public IncidentIqUserClient(IHttpClientFactory httpClientFactory, ILogger<IncidentIqUserClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IncidentIqFetchResult> GetUserAsync(IncidentIqConnectionSettings settings, string userId, CancellationToken cancellationToken = default)
    {
        using var request = BuildRequest(settings, HttpMethod.Get, userId);

        using var timeoutCts = new CancellationTokenSource(RequestTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            var client = _httpClientFactory.CreateClient(HttpCallExecutor.HttpClientName);
            using var response = await client.SendAsync(request, linkedCts.Token);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new IncidentIqFetchResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    RawResponse = Truncate(body),
                    ErrorMessage = $"Incident IQ returned {(int)response.StatusCode}.",
                };
            }

            JsonObject? user;
            try
            {
                user = JsonNode.Parse(body)?.AsObject();
            }
            catch (JsonException ex)
            {
                return new IncidentIqFetchResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    RawResponse = Truncate(body),
                    ErrorMessage = $"Response wasn't valid JSON: {ex.Message}",
                };
            }

            if (user is null)
            {
                return new IncidentIqFetchResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    RawResponse = Truncate(body),
                    ErrorMessage = "Response wasn't a JSON object.",
                };
            }

            return new IncidentIqFetchResult { Success = true, StatusCode = (int)response.StatusCode, User = user, RawResponse = body };
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new IncidentIqFetchResult { Success = false, ErrorMessage = $"Timed out after {RequestTimeout.TotalSeconds:0}s." };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Failed to fetch Incident IQ user {UserId}.", userId);
            return new IncidentIqFetchResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<IncidentIqSaveResult> SaveUserAsync(IncidentIqConnectionSettings settings, string userId, JsonObject updatedUser, CancellationToken cancellationToken = default)
    {
        using var request = BuildRequest(settings, HttpMethod.Post, userId);
        request.Content = new StringContent(updatedUser.ToJsonString(), Encoding.UTF8, "application/json");

        using var timeoutCts = new CancellationTokenSource(RequestTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            var client = _httpClientFactory.CreateClient(HttpCallExecutor.HttpClientName);
            using var response = await client.SendAsync(request, linkedCts.Token);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            return new IncidentIqSaveResult
            {
                Success = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                ResponseSnippet = Truncate(body),
                ErrorMessage = response.IsSuccessStatusCode ? null : $"Incident IQ returned {(int)response.StatusCode}.",
            };
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new IncidentIqSaveResult { Success = false, ErrorMessage = $"Timed out after {RequestTimeout.TotalSeconds:0}s." };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Failed to save Incident IQ user {UserId}.", userId);
            return new IncidentIqSaveResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    private static HttpRequestMessage BuildRequest(IncidentIqConnectionSettings settings, HttpMethod method, string userId)
    {
        var baseUrl = settings.ApiBaseUrl.TrimEnd('/');
        var request = new HttpRequestMessage(method, $"{baseUrl}/users/{Uri.EscapeDataString(userId)}");
        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {settings.ApiToken}");
        request.Headers.TryAddWithoutValidation("SiteId", settings.SiteId);
        request.Headers.TryAddWithoutValidation("Client", "ApiClient");
        return request;
    }

    private static string? Truncate(string? value) =>
        string.IsNullOrEmpty(value) || value.Length <= MaxResponseSnippetLength
            ? value
            : string.Concat(value.AsSpan(0, MaxResponseSnippetLength), "… (truncated)");
}
