// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using TempMailBot.Logging.Enums;
using TempMailBot.Logging.Models;

namespace TempMailBot.Logging.Filters;

/// <summary>
/// A filter that filters log entries based on their log level.
/// </summary>
/// <remarks>
/// This filter only allows log entries with a level greater than or equal to
/// the specified minimum level to pass through.
/// </remarks>
public class LevelFilter : ILogFilter
{
    private readonly LogLevel _minLevel;

    /// <summary>
    /// Initializes a new instance of the <see cref="LevelFilter"/> class.
    /// </summary>
    /// <param name="minLevel">The minimum log level to allow through the filter.</param>
    public LevelFilter(LogLevel minLevel)
    {
        _minLevel = minLevel;
    }

    /// <summary>
    /// Gets the name of the filter.
    /// </summary>
    /// <value>The name of the filter including the minimum level.</value>
    public string Name => $"LevelFilter({_minLevel})";

    /// <summary>
    /// Determines whether the specified log entry should be processed based on its level.
    /// </summary>
    /// <param name="logEntry">The log entry to evaluate.</param>
    /// <returns>True if the log entry level is greater than or equal to the minimum level; otherwise, false.</returns>
    public bool IsEnabled(LogEntry logEntry)
    {
        if (logEntry == null)
            return false;

        return logEntry.Level >= _minLevel;
    }
}
