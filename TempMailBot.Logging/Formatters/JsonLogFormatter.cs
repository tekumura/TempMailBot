// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text.Json;
using TempMailBot.Logging.Models;

namespace TempMailBot.Logging.Formatters;

/// <summary>
/// A log formatter that formats log entries as JSON.
/// </summary>
/// <remarks>
/// This formatter converts log entries into JSON format, which is useful for
/// structured logging and integration with log aggregation systems.
/// </remarks>
public class JsonLogFormatter : ILogFormatter
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonLogFormatter"/> class.
    /// </summary>
    /// <param name="options">The JSON serializer options. If null, default options are used.</param>
    public JsonLogFormatter(JsonSerializerOptions? options = null)
    {
        _options = options ?? new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Formats a log entry as JSON using the default template.
    /// </summary>
    /// <param name="logEntry">The log entry to format.</param>
    /// <returns>A JSON string representation of the log entry.</returns>
    public string Format(LogEntry logEntry)
    {
        return Format(logEntry, string.Empty);
    }

    /// <summary>
    /// Formats a log entry as JSON using a custom template.
    /// </summary>
    /// <param name="logEntry">The log entry to format.</param>
    /// <param name="template">The template parameter is ignored for JSON formatting.</param>
    /// <returns>A JSON string representation of the log entry.</returns>
    /// <exception cref="ArgumentNullException">Thrown when logEntry is null.</exception>
    public string Format(LogEntry logEntry, string template)
    {
        if (logEntry == null)
            throw new ArgumentNullException(nameof(logEntry));

        var logObject = new
        {
            Timestamp = logEntry.Timestamp,
            Level = logEntry.Level.ToString(),
            LoggerName = logEntry.LoggerName,
            Message = logEntry.Message,
            Exception = logEntry.Exception?.ToString(),
            Properties = logEntry.Properties
        };

        return JsonSerializer.Serialize(logObject, _options);
    }
}