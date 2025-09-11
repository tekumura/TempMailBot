// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using TempMailBot.Logging.Formatters;
using TempMailBot.Logging.Models;

namespace TempMailBot.Logging.Handlers;

/// <summary>
/// A log handler that outputs log entries to files with rotation support.
/// </summary>
/// <remarks>
/// This handler writes log entries to files with automatic rotation when files reach
/// a specified maximum size. It supports multiple backup files and creates the log
/// directory if it doesn't exist.
/// </remarks>
public class FileLogHandler : ILogHandler
{
    private readonly string _logDir;
    private ILogFormatter _formatter;
    private readonly int _maxFileSize;
    private readonly int _backupCount;
    private readonly object _lock = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="FileLogHandler"/> class.
    /// </summary>
    /// <param name="logDir">The directory where log files will be stored.</param>
    /// <param name="formatter">The formatter to use for formatting log entries.</param>
    /// <param name="maxFileSize">The maximum file size in bytes before rotation. Default is 10MB.</param>
    /// <param name="backupCount">The number of backup files to keep. Default is 5.</param>
    /// <exception cref="ArgumentNullException">Thrown when logDir or formatter is null.</exception>
    public FileLogHandler(string logDir, ILogFormatter formatter, int maxFileSize = 10 * 1024 * 1024, int backupCount = 5)
    {
        _logDir = logDir ?? throw new ArgumentNullException(nameof(logDir));
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        _maxFileSize = maxFileSize;
        _backupCount = backupCount;

        if (!Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this handler is enabled.
    /// </summary>
    /// <value>True if the handler is enabled; otherwise, false.</value>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Handles a log entry by writing it to a file with rotation support.
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
                var logFile = Path.Combine(_logDir, $"{DateTime.Now:yyyy-MM-dd}.log");
                var formattedMessage = _formatter.Format(logEntry);

                if (!File.Exists(logFile))
                {
                    using var fs = File.Create(logFile);
                }

                if (new FileInfo(logFile).Length >= _maxFileSize)
                {
                    RotateLogs(logFile);
                }

                File.AppendAllText(logFile, formattedMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Log the error to console to avoid infinite loops
                Console.WriteLine($"Error writing to log file: {ex.Message}");
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
    /// Rotates log files when they reach the maximum size.
    /// </summary>
    /// <param name="logFile">The current log file to rotate.</param>
    private void RotateLogs(string logFile)
    {
        try
        {
            for (int i = _backupCount - 1; i >= 0; i--)
            {
                var source = i == 0 ? logFile : $"{logFile}.{i}";
                var dest = $"{logFile}.{i + 1}";
                
                if (File.Exists(source))
                {
                    if (File.Exists(dest))
                    {
                        File.Delete(dest);
                    }
                    File.Move(source, dest);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rotating log files: {ex.Message}");
        }
    }
}
