// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace TempMailBot.Logging.Exceptions;

/// <summary>
/// Represents errors that occur during formatter processing.
/// </summary>
public class FormatterException : LoggingException
{
    /// <summary>Initializes a new instance of the <see cref="FormatterException"/> class.</summary>
    public FormatterException() : base()
    {
    }

    /// <summary>Initializes a new instance with a specified error message.</summary>
    public FormatterException(string message) : base(message)
    {
    }

    /// <summary>Initializes a new instance with a specified error message and inner exception.</summary>
    public FormatterException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance with a formatter name and message.</summary>
    public FormatterException(string formatterName, string message) : base(message)
    {
        FormatterName = formatterName;
    }

    /// <summary>Initializes a new instance with a formatter name, message and inner exception.</summary>
    public FormatterException(string formatterName, string message, Exception innerException) : base(message, innerException)
    {
        FormatterName = formatterName;
    }
}
