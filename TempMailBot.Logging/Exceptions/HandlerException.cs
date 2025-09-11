// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace TempMailBot.Logging.Exceptions;

/// <summary>
/// Represents errors originating from log handlers.
/// </summary>
public class HandlerException : LoggingException
{
    /// <summary>Initializes a new instance of the <see cref="HandlerException"/> class.</summary>
    public HandlerException() : base()
    {
    }

    /// <summary>Initializes a new instance with a specified error message.</summary>
    public HandlerException(string message) : base(message)
    {
    }

    /// <summary>Initializes a new instance with a specified error message and inner exception.</summary>
    public HandlerException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance with a handler name and message.</summary>
    public HandlerException(string handlerName, string message) : base(message)
    {
        HandlerName = handlerName;
    }

    /// <summary>Initializes a new instance with a handler name, message and inner exception.</summary>
    public HandlerException(string handlerName, string message, Exception innerException) : base(message, innerException)
    {
        HandlerName = handlerName;
    }
}
