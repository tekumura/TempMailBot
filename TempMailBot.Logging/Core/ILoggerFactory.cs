// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace TempMailBot.Logging.Core;

/// <summary>
/// Represents a factory for creating logger instances.
/// </summary>
/// <remarks>
/// This interface provides methods for creating loggers with specific names or types,
/// and managing logger providers.
/// </remarks>
public interface ILoggerFactory : IDisposable
{
    /// <summary>
    /// Creates a logger with the specified name.
    /// </summary>
    /// <param name="name">The name of the logger.</param>
    /// <returns>A logger instance.</returns>
    ILogger CreateLogger(string name);
    
    /// <summary>
    /// Creates a logger for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to create a logger for.</typeparam>
    /// <returns>A logger instance.</returns>
    ILogger CreateLogger<T>();
    
    /// <summary>
    /// Adds a logger provider to the factory.
    /// </summary>
    /// <param name="provider">The provider to add.</param>
    void AddProvider(ILoggerProvider provider);
    
    /// <summary>
    /// Removes a logger provider from the factory.
    /// </summary>
    /// <param name="provider">The provider to remove.</param>
    void RemoveProvider(ILoggerProvider provider);
}

/// <summary>
/// Represents a provider that creates logger instances.
/// </summary>
/// <remarks>
/// This interface is used by the logger factory to create logger instances
/// for specific categories or types.
/// </remarks>
public interface ILoggerProvider : IDisposable
{
    /// <summary>
    /// Creates a logger for the specified category name.
    /// </summary>
    /// <param name="categoryName">The category name for the logger.</param>
    /// <returns>A logger instance.</returns>
    ILogger CreateLogger(string categoryName);
}
