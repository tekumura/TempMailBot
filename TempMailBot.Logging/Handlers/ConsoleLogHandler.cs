// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using TempMailBot.Logging.Formatters;
using TempMailBot.Logging.Models;

namespace TempMailBot.Logging.Handlers;

/// <summary>
/// A log handler that outputs log entries to the console with color coding.
/// </summary>
/// <remarks>
/// This handler writes log entries to the console output with different colors
/// for different log levels to improve readability.
/// </remarks>
public class ConsoleLogHandler : ILogHandler
{
    private ILogFormatter _formatter;
    private readonly object _lock = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleLogHandler"/> class.
    /// </summary>
    /// <param name="formatter">The formatter to use for formatting log entries.</param>
    /// <exception cref="ArgumentNullException">Thrown when formatter is null.</exception>
    public ConsoleLogHandler(ILogFormatter formatter)
    {
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    /// <summary>
    /// Gets or sets a value indicating whether this handler is enabled.
    /// </summary>
    /// <value>True if the handler is enabled; otherwise, false.</value>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Handles a log entry by writing it to the console with color coding.
    /// </summary>
    /// <param name="logEntry">The log entry to handle.</param>
    public void Handle(LogEntry logEntry)
    {
        if (!IsEnabled)
            return;

        lock (_lock)
        {
            try
            {
                var formattedMessage = _formatter.Format(logEntry);
                
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = GetColorForLevel(logEntry.Level);
                
                Console.WriteLine(formattedMessage);
                
                Console.ForegroundColor = originalColor;
            }
            catch (Exception)
            {
                Console.WriteLine($"[{logEntry.Level}] {logEntry.LoggerName}: {logEntry.Message}");
                if (logEntry.Exception != null)
                {
                    Console.WriteLine($"Exception: {logEntry.Exception}");
                }
            }
        }
    }

    /// <summary>
    /// Sets the formatter to use for formatting log entries.
    /// </summary>
    /// <param name="formatter">The formatter to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when formatter is null.</exception>
    public void SetFormatter(ILogFormatter formatter)
    {
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    /// <summary>
    /// Gets the console color for the specified log level.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <returns>The console color for the log level.</returns>
    private static ConsoleColor GetColorForLevel(Enums.LogLevel level)
    {
        return level switch
        {
            Enums.LogLevel.Trace => ConsoleColor.Gray,
            Enums.LogLevel.Debug => ConsoleColor.Cyan,
            Enums.LogLevel.Info => ConsoleColor.White,
            Enums.LogLevel.Warning => ConsoleColor.Yellow,
            Enums.LogLevel.Error => ConsoleColor.Red,
            Enums.LogLevel.Critical => ConsoleColor.Magenta,
            _ => ConsoleColor.White
        };
    }
}
