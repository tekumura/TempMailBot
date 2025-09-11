// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text;
using System.Text.RegularExpressions;
using TempMailBot.Logging.Models;

namespace TempMailBot.Logging.Formatters;

/// <summary>
/// A structured log formatter that provides advanced formatting with configurable options.
/// </summary>
/// <remarks>
/// This formatter provides structured logging capabilities with configurable templates
/// and options for including exceptions and properties in the output.
/// </remarks>
public class StructuredLogFormatter : ILogFormatter
{
    private readonly string _template;
    private readonly bool _includeException;
    private readonly bool _includeProperties;

    /// <summary>
    /// Initializes a new instance of the <see cref="StructuredLogFormatter"/> class.
    /// </summary>
    /// <param name="template">The template to use for formatting. Default includes timestamp, level, logger name, and message.</param>
    /// <param name="includeException">Whether to include exception information in the output.</param>
    /// <param name="includeProperties">Whether to include properties in the output.</param>
    /// <exception cref="ArgumentNullException">Thrown when template is null.</exception>
    public StructuredLogFormatter(string template = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {LoggerName}: {Message}", 
                                 bool includeException = true, 
                                 bool includeProperties = true)
    {
        _template = template ?? throw new ArgumentNullException(nameof(template));
        _includeException = includeException;
        _includeProperties = includeProperties;
    }

    /// <summary>
    /// Formats a log entry using the configured template.
    /// </summary>
    /// <param name="logEntry">The log entry to format.</param>
    /// <returns>A formatted string representation of the log entry.</returns>
    public string Format(LogEntry logEntry)
    {
        return Format(logEntry, _template);
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

        var result = new StringBuilder();
        var currentTemplate = template;

        // Replace Timestamp with optional format {Timestamp:format}
        currentTemplate = Regex.Replace(currentTemplate, "\\{Timestamp(?::([^}]+))?\\}", m =>
        {
            var fmt = m.Groups[1].Success ? m.Groups[1].Value : "yyyy-MM-dd HH:mm:ss";
            return logEntry.Timestamp.ToString(fmt);
        });

        // Replace basic placeholders
        currentTemplate = currentTemplate
            .Replace("{Level}", logEntry.Level.ToString())
            .Replace("{LoggerName}", logEntry.LoggerName)
            .Replace("{Message}", logEntry.Message);

        result.Append(currentTemplate);

        // Add exception if present and enabled
        if (_includeException && logEntry.Exception != null)
        {
            result.AppendLine();
            result.AppendLine($"Exception: {logEntry.Exception}");
        }

        // Add properties if present and enabled
        if (_includeProperties && logEntry.Properties.Any())
        {
            result.AppendLine();
            result.Append("Properties: ");
            result.Append(string.Join(", ", logEntry.Properties.Select(p => $"{p.Key}={FormatValue(p.Value)}")));
        }

        return result.ToString();
    }

    /// <summary>
    /// Formats a value for inclusion in the log output.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>A formatted string representation of the value.</returns>
    private static string FormatValue(object value)
    {
        if (value == null)
            return "null";

        if (value is string str)
            return $"\"{str}\"";

        if (value is DateTime dt)
            return $"\"{dt:yyyy-MM-dd HH:mm:ss}\"";

        if (value is Exception ex)
            return $"\"{ex.GetType().Name}: {ex.Message}\"";

        return value.ToString() ?? "null";
    }
}