// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using TempMailBot.Logging.Models;

namespace TempMailBot.Logging.Filters;

/// <summary>
/// Represents a filter that can determine whether a log entry should be processed.
/// </summary>
/// <remarks>
/// This interface defines the contract for filters that can evaluate log entries
/// and determine whether they should be processed by handlers.
/// </remarks>
public interface ILogFilter
{
    /// <summary>
    /// Determines whether the specified log entry should be processed.
    /// </summary>
    /// <param name="logEntry">The log entry to evaluate.</param>
    /// <returns>True if the log entry should be processed; otherwise, false.</returns>
    bool IsEnabled(LogEntry logEntry);
    
    /// <summary>
    /// Gets the name of the filter.
    /// </summary>
    /// <value>The name of the filter.</value>
    string Name { get; }
}
