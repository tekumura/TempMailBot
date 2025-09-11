// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using TempMailBot.Logging.Enums;

namespace TempMailBot.Logging.Models;

/// <summary>
/// Represents a log entry with all the necessary information for logging.
/// </summary>
/// <remarks>
/// This class contains all the information needed to log a message, including the logger name,
/// log level, message, exception, timestamp, and additional properties.
/// </remarks>
public class LogEntry
{
    /// <summary>
    /// Gets the name of the logger that created this entry.
    /// </summary>
    /// <value>The logger name.</value>
    public string LoggerName { get; }
    
    /// <summary>
    /// Gets the log level of this entry.
    /// </summary>
    /// <value>The log level.</value>
    public LogLevel Level { get; }
    
    /// <summary>
    /// Gets the log message.
    /// </summary>
    /// <value>The log message.</value>
    public string Message { get; }
    
    /// <summary>
    /// Gets the exception associated with this log entry, if any.
    /// </summary>
    /// <value>The exception, or null if no exception is associated.</value>
    public Exception? Exception { get; }
    
    /// <summary>
    /// Gets the timestamp when this log entry was created.
    /// </summary>
    /// <value>The timestamp.</value>
    public DateTime Timestamp { get; }
    
    /// <summary>
    /// Gets the additional properties associated with this log entry.
    /// </summary>
    /// <value>A read-only dictionary of properties.</value>
    public IReadOnlyDictionary<string, object> Properties { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class.
    /// </summary>
    /// <param name="loggerName">The name of the logger.</param>
    /// <param name="level">The log level.</param>
    /// <param name="message">The log message.</param>
    /// <param name="exception">The exception, if any.</param>
    /// <param name="timestamp">The timestamp. If null, uses the current UTC time.</param>
    /// <param name="properties">Additional properties for the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown when loggerName or message is null.</exception>
    public LogEntry(string loggerName, LogLevel level, string message, Exception? exception = null, DateTime? timestamp = null, IDictionary<string, object>? properties = null)
    {
        LoggerName = loggerName ?? throw new ArgumentNullException(nameof(loggerName));
        Level = level;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Exception = exception;
        Timestamp = timestamp ?? DateTime.UtcNow;
        Properties = properties != null 
            ? new Dictionary<string, object>(properties) 
            : new Dictionary<string, object>();
    }

    /// <summary>
    /// Returns a string representation of the log entry.
    /// </summary>
    /// <returns>A formatted string representation of the log entry.</returns>
    public override string ToString()
    {
        return $"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {LoggerName}: {Message}";
    }
}
