// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using TempMailBot.Logging.Models;

namespace TempMailBot.Logging.Filters;

/// <summary>
/// A filter that filters log entries based on the time of day.
/// </summary>
/// <remarks>
/// This filter only allows log entries that are created within a specified
/// time range to pass through.
/// </remarks>
public class TimeFilter : ILogFilter
{
    private readonly TimeSpan _startTime;
    private readonly TimeSpan _endTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeFilter"/> class.
    /// </summary>
    /// <param name="startTime">The start time of the allowed time range.</param>
    /// <param name="endTime">The end time of the allowed time range.</param>
    public TimeFilter(TimeSpan startTime, TimeSpan endTime)
    {
        _startTime = startTime;
        _endTime = endTime;
    }

    /// <summary>
    /// Gets the name of the filter.
    /// </summary>
    /// <value>The name of the filter including the time range.</value>
    public string Name => $"TimeFilter({_startTime}-{_endTime})";

    /// <summary>
    /// Determines whether the specified log entry should be processed based on its timestamp.
    /// </summary>
    /// <param name="logEntry">The log entry to evaluate.</param>
    /// <returns>True if the log entry timestamp is within the allowed time range; otherwise, false.</returns>
    public bool IsEnabled(LogEntry logEntry)
    {
        if (logEntry == null)
            return false;

        var now = logEntry.Timestamp.TimeOfDay;
        return now >= _startTime && now <= _endTime;
    }
}
