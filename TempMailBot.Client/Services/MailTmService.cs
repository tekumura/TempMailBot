// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Net.Http.Json;
using TempMailBot.Logging.Core;

namespace TempMailBot.Client.Services;

/// <summary>
/// Provides asynchronous methods to interact with Mail.tm API.
/// </summary>
public class MailTmService
{
    private readonly HttpClient _http;
    private readonly ILogger _logger;
    private const string BaseUrl = "https://api.mail.tm";

    /// <summary>
    /// Initializes a new instance of <see cref="MailTmService"/>.
    /// </summary>
    /// <param name="logger">Shared logger instance.</param>
    public MailTmService(ILogger logger)
    {
        _http = new HttpClient();
        _logger = logger;
    }

    /// <summary>
    /// Returns a list of active Mail.tm domains.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of domain strings.</returns>
    public async Task<List<string>> GetDomainsAsync(CancellationToken ct)
    {
        try
        {
            var resp = await _http.GetAsync($"{BaseUrl}/domains", ct);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, object>>(cancellationToken: ct);
            if (json != null && json.TryGetValue("hydra:member", out var memberObj) && memberObj is System.Text.Json.JsonElement el && el.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                var list = new List<string>();
                foreach (var item in el.EnumerateArray())
                {
                    var isActive = item.TryGetProperty("isActive", out var activeEl) && activeEl.GetBoolean();
                    if (isActive && item.TryGetProperty("domain", out var domainEl))
                        list.Add(domainEl.GetString()!);
                }
                return list;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get mail.tm domains: {0}", ex.Message);
        }
        return new List<string>();
    }

    /// <summary>
    /// Creates a Mail.tm account.
    /// </summary>
    /// <param name="address">Email address.</param>
    /// <param name="password">Password.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True on success.</returns>
    public async Task<bool> CreateAccountAsync(string address, string password, CancellationToken ct)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync($"{BaseUrl}/accounts", new { address, password }, ct);
            if (resp.IsSuccessStatusCode) return true;
            _logger.LogWarning("Mail.tm account create failed for {0}: {1}", address, (int)resp.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create account: {0}", ex.Message);
        }
        return false;
    }

    /// <summary>
    /// Authenticates and returns a JWT token for subsequent API calls.
    /// </summary>
    /// <param name="address">Email address.</param>
    /// <param name="password">Password.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>JWT token or null.</returns>
    public async Task<string?> AuthenticateAsync(string address, string password, CancellationToken ct)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync($"{BaseUrl}/token", new { address, password }, ct);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, object>>(cancellationToken: ct);
            if (json != null && json.TryGetValue("token", out var tokenObj))
                return tokenObj?.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to authenticate: {0}", ex.Message);
        }
        return null;
    }

    /// <summary>
    /// Returns list of messages for the authenticated account.
    /// </summary>
    /// <param name="token">JWT token.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of messages as dictionaries.</returns>
    public async Task<List<Dictionary<string, object>>> GetMessagesAsync(string token, CancellationToken ct)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/messages");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var resp = await _http.SendAsync(req, ct);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, object>>(cancellationToken: ct);
            if (json != null && json.TryGetValue("hydra:member", out var memberObj) && memberObj is System.Text.Json.JsonElement el && el.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                var list = new List<Dictionary<string, object>>();
                foreach (var item in el.EnumerateArray())
                {
                    var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(item.GetRawText());
                    if (dict != null) list.Add(dict);
                }
                return list;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get messages: {0}", ex.Message);
        }
        return new List<Dictionary<string, object>>();
    }
}


