// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using TempMailBot.Logging.Models;

namespace TempMailBot.Logging.Formatters;

/// <summary>
/// Represents a formatter that can format log entries into strings.
/// </summary>
/// <remarks>
/// This interface defines the contract for formatters that can convert log entries
/// into formatted strings for output to various destinations.
/// </remarks>
public interface ILogFormatter
{
    /// <summary>
    /// Formats a log entry into a string using the default template.
    /// </summary>
    /// <param name="logEntry">The log entry to format.</param>
    /// <returns>A formatted string representation of the log entry.</returns>
    string Format(LogEntry logEntry);
    
    /// <summary>
    /// Formats a log entry into a string using a custom template.
    /// </summary>
    /// <param name="logEntry">The log entry to format.</param>
    /// <param name="template">The template to use for formatting.</param>
    /// <returns>A formatted string representation of the log entry.</returns>
    string Format(LogEntry logEntry, string template);
}
