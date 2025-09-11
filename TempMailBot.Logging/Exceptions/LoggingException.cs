// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace TempMailBot.Logging.Exceptions;

/// <summary>
/// Base exception for logging-related errors.
/// </summary>
public class LoggingException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="LoggingException"/> class.</summary>
    public LoggingException() : base()
    {
    }

    /// <summary>Initializes a new instance with a specified error message.</summary>
    public LoggingException(string message) : base(message)
    {
    }

    /// <summary>Initializes a new instance with a specified error message and inner exception.</summary>
    public LoggingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>The logger name associated with the error, if any.</summary>
    public string? LoggerName { get; set; }
    /// <summary>The handler name associated with the error, if any.</summary>
    public string? HandlerName { get; set; }
    /// <summary>The formatter name associated with the error, if any.</summary>
    public string? FormatterName { get; set; }
    /// <summary>The filter name associated with the error, if any.</summary>
    public string? FilterName { get; set; }
}
