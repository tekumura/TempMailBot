// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace TempMailBot.Logging.Exceptions;

/// <summary>
/// Represents errors that occur during filter processing.
/// </summary>
public class FilterException : LoggingException
{
    /// <summary>Initializes a new instance of the <see cref="FilterException"/> class.</summary>
    public FilterException() : base()
    {
    }

    /// <summary>Initializes a new instance with a specified error message.</summary>
    public FilterException(string message) : base(message)
    {
    }

    /// <summary>Initializes a new instance with a specified error message and inner exception.</summary>
    public FilterException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance with a filter name and message.</summary>
    public FilterException(string filterName, string message) : base(message)
    {
        FilterName = filterName;
    }

    /// <summary>Initializes a new instance with a filter name, message and inner exception.</summary>
    public FilterException(string filterName, string message, Exception innerException) : base(message, innerException)
    {
        FilterName = filterName;
    }
}
