// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using TempMailBot.Logging.Formatters;
using TempMailBot.Logging.Models;

namespace TempMailBot.Logging.Handlers;

/// <summary>
/// Represents a handler that processes log entries.
/// </summary>
/// <remarks>
/// This interface defines the contract for log handlers that can process log entries
/// and output them to various destinations (console, file, database, etc.).
/// </remarks>
public interface ILogHandler
{
    /// <summary>
    /// Handles a log entry by processing and outputting it.
    /// </summary>
    /// <param name="logEntry">The log entry to handle.</param>
    void Handle(LogEntry logEntry);
    
    /// <summary>
    /// Sets the formatter to use for formatting log entries.
    /// </summary>
    /// <param name="formatter">The formatter to use.</param>
    void SetFormatter(ILogFormatter formatter);
    
    /// <summary>
    /// Gets or sets a value indicating whether this handler is enabled.
    /// </summary>
    /// <value>True if the handler is enabled; otherwise, false.</value>
    bool IsEnabled { get; set; }
}
