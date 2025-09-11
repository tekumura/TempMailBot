// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using TempMailBot.Logging.Models;
using System.Text.RegularExpressions;

namespace TempMailBot.Logging.Formatters;

/// <summary>
/// A default log formatter that formats log entries using a simple template.
/// </summary>
/// <remarks>
/// This formatter provides a basic formatting implementation that includes
/// timestamp, log level, logger name, message, exception, and properties.
/// </remarks>
public class DefaultLogFormatter : ILogFormatter
{
    private const string DefaultTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {LoggerName}: {Message}";

    /// <summary>
    /// Formats a log entry using the default template.
    /// </summary>
    /// <param name="logEntry">The log entry to format.</param>
    /// <returns>A formatted string representation of the log entry.</returns>
    public string Format(LogEntry logEntry)
    {
        return Format(logEntry, DefaultTemplate);
    }

    /// <summary>
    /// Formats a log entry using a custom template.
    /// </summary>
    /// <param name="logEntry">The log entry to format.</param>
    /// <param name="template">The template to use for formatting.</param>
    /// <returns>A formatted string representation of the log entry.</returns>
    /// <exception cref="ArgumentNullException">Thrown when logEntry is null.</exception>
    /// <exception cref="ArgumentException">Thrown when template is null or empty.</exception>
    public string Format(LogEntry logEntry, string template)
    {
        if (logEntry == null)
            throw new ArgumentNullException(nameof(logEntry));

        if (string.IsNullOrEmpty(template))
            throw new ArgumentException("Template cannot be null or empty", nameof(template));

        // Replace Timestamp with optional custom format: {Timestamp:format}
        var result = Regex.Replace(template, "\\{Timestamp(?::([^}]+))?\\}", m =>
        {
            var fmt = m.Groups[1].Success ? m.Groups[1].Value : "yyyy-MM-dd HH:mm:ss";
            return logEntry.Timestamp.ToString(fmt);
        });

        result = result
            .Replace("{Level}", logEntry.Level.ToString())
            .Replace("{LoggerName}", logEntry.LoggerName)
            .Replace("{Message}", logEntry.Message);

        if (logEntry.Exception != null)
        {
            result += $"\nException: {logEntry.Exception}";
        }

        if (logEntry.Properties.Any())
        {
            result += $"\nProperties: {string.Join(", ", logEntry.Properties.Select(p => $"{p.Key}={p.Value}"))}";
        }

        return result;
    }
}
