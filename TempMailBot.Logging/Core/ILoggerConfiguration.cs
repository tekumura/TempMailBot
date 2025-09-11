// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using TempMailBot.Logging.Enums;
using TempMailBot.Logging.Handlers;
using TempMailBot.Logging.Filters;

namespace TempMailBot.Logging.Core;

/// <summary>
/// Represents the configuration for a logger.
/// </summary>
/// <remarks>
/// This interface provides methods for configuring log levels, handlers, and filters
/// for logger instances.
/// </remarks>
public interface ILoggerConfiguration
{
    /// <summary>
    /// Gets or sets the minimum log level.
    /// </summary>
    /// <value>The minimum log level.</value>
    LogLevel MinimumLevel { get; set; }
    
    /// <summary>
    /// Gets the list of log handlers.
    /// </summary>
    /// <value>The list of log handlers.</value>
    IList<ILogHandler> Handlers { get; }
    
    /// <summary>
    /// Gets the list of log filters.
    /// </summary>
    /// <value>The list of log filters.</value>
    IList<ILogFilter> Filters { get; }
    
    /// <summary>
    /// Adds a log handler to the configuration.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    void AddHandler(ILogHandler handler);
    
    /// <summary>
    /// Removes a log handler from the configuration.
    /// </summary>
    /// <param name="handler">The handler to remove.</param>
    void RemoveHandler(ILogHandler handler);
    
    /// <summary>
    /// Adds a log filter to the configuration.
    /// </summary>
    /// <param name="filter">The filter to add.</param>
    void AddFilter(ILogFilter filter);
    
    /// <summary>
    /// Removes a log filter from the configuration.
    /// </summary>
    /// <param name="filter">The filter to remove.</param>
    void RemoveFilter(ILogFilter filter);
    
    /// <summary>
    /// Sets the minimum log level for all categories.
    /// </summary>
    /// <param name="level">The minimum log level.</param>
    void SetMinimumLevel(LogLevel level);
    
    /// <summary>
    /// Sets the minimum log level for a specific category.
    /// </summary>
    /// <param name="category">The category name.</param>
    /// <param name="level">The minimum log level.</param>
    void SetMinimumLevel(string category, LogLevel level);
}
