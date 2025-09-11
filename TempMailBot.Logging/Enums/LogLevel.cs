// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace TempMailBot.Logging.Enums;

/// <summary>
/// Defines the logging levels.
/// </summary>
/// <remarks>
/// Log levels are ordered from least to most severe: Trace, Debug, Info, Warning, Error, Critical.
/// When a minimum level is set, only messages at that level or higher will be logged.
/// </remarks>
public enum LogLevel
{ 
    /// <summary>
    /// Trace level - very detailed information, typically only of interest when diagnosing problems.
    /// </summary>
    Trace,
    
    /// <summary>
    /// Debug level - detailed information for debugging purposes.
    /// </summary>
    Debug,
    
    /// <summary>
    /// Info level - general information about the application flow.
    /// </summary>
    Info,
    
    /// <summary>
    /// Warning level - something unexpected happened, but the application is still working.
    /// </summary>
    Warning,
    
    /// <summary>
    /// Error level - an error occurred, but the application can continue.
    /// </summary>
    Error,
    
    /// <summary>
    /// Critical level - a critical error occurred that may cause the application to terminate.
    /// </summary>
    Critical
}