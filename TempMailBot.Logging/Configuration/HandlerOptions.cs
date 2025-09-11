// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace TempMailBot.Logging.Configuration;

/// <summary>
/// Console handler options.
/// </summary>
public class ConsoleHandlerOptions : HandlerOptions
{
    public ConsoleHandlerOptions()
    {
        Type = "Console";
    }

    /// <summary>Enables ANSI colors in console output.</summary>
    public bool UseColors { get; set; } = true;
    /// <summary>Includes timestamp in console output.</summary>
    public bool IncludeTimestamp { get; set; } = true;
    /// <summary>Includes log level in console output.</summary>
    public bool IncludeLevel { get; set; } = true;
    /// <summary>Includes logger name in console output.</summary>
    public bool IncludeLoggerName { get; set; } = true;
}

/// <summary>
/// File handler options.
/// </summary>
public class FileHandlerOptions : HandlerOptions
{
    public FileHandlerOptions()
    {
        Type = "File";
    }

    /// <summary>Directory for log files.</summary>
    public string LogDirectory { get; set; } = "logs";
    /// <summary>Template for file names (e.g., {Date}.log).</summary>
    public string FileNameTemplate { get; set; } = "{Date}.log";
    /// <summary>Max file size in bytes before rotation.</summary>
    public int MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
    /// <summary>Number of rotated backups to keep.</summary>
    public int MaxBackupCount { get; set; } = 5;
    /// <summary>Create directory if it does not exist.</summary>
    public bool CreateDirectoryIfNotExists { get; set; } = true;
    /// <summary>Include timestamp for each line.</summary>
    public bool IncludeTimestamp { get; set; } = true;
}

/// <summary>
/// Database handler options.
/// </summary>
public class DatabaseHandlerOptions : HandlerOptions
{
    public DatabaseHandlerOptions()
    {
        Type = "Database";
    }

    /// <summary>Connection string to the database.</summary>
    public string ConnectionString { get; set; } = string.Empty;
    /// <summary>Table name to store logs.</summary>
    public string TableName { get; set; } = "Logs";
    /// <summary>Create table if it does not exist.</summary>
    public bool CreateTableIfNotExists { get; set; } = true;
    /// <summary>Batch size for buffered writes.</summary>
    public int BatchSize { get; set; } = 100;
    /// <summary>Flush interval for buffered writes.</summary>
    public TimeSpan FlushInterval { get; set; } = TimeSpan.FromSeconds(5);
}
