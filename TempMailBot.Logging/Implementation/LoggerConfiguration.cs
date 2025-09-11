// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using TempMailBot.Logging.Core;
using TempMailBot.Logging.Enums;
using TempMailBot.Logging.Filters;
using TempMailBot.Logging.Handlers;

namespace TempMailBot.Logging.Implementation;

/// <summary>
/// Provides a concrete implementation of the <see cref="ILoggerConfiguration"/> interface.
/// </summary>
/// <remarks>
/// This configuration class manages log levels, handlers, and filters for the logging system.
/// It supports category-specific log levels and provides thread-safe access to configuration data.
/// </remarks>
public class LoggerConfiguration : ILoggerConfiguration
{
    private readonly ConcurrentDictionary<string, LogLevel> _categoryLevels;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerConfiguration"/> class.
    /// </summary>
    public LoggerConfiguration()
    {
        MinimumLevel = LogLevel.Info;
        Handlers = new List<ILogHandler>();
        Filters = new List<ILogFilter>();
        _categoryLevels = new ConcurrentDictionary<string, LogLevel>();
    }

    /// <summary>
    /// Gets or sets the minimum log level for the configuration.
    /// </summary>
    /// <value>The minimum log level.</value>
    public LogLevel MinimumLevel { get; set; }
    
    /// <summary>
    /// Gets the list of log handlers.
    /// </summary>
    /// <value>A list of log handlers.</value>
    public IList<ILogHandler> Handlers { get; }
    
    /// <summary>
    /// Gets the list of log filters.
    /// </summary>
    /// <value>A list of log filters.</value>
    public IList<ILogFilter> Filters { get; }

    /// <summary>
    /// Adds a log handler to the configuration.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when handler is null.</exception>
    public void AddHandler(ILogHandler handler)
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        Handlers.Add(handler);
    }

    /// <summary>
    /// Removes a log handler from the configuration.
    /// </summary>
    /// <param name="handler">The handler to remove.</param>
    /// <exception cref="ArgumentNullException">Thrown when handler is null.</exception>
    public void RemoveHandler(ILogHandler handler)
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        Handlers.Remove(handler);
    }

    /// <summary>
    /// Adds a log filter to the configuration.
    /// </summary>
    /// <param name="filter">The filter to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when filter is null.</exception>
    public void AddFilter(ILogFilter filter)
    {
        if (filter == null)
            throw new ArgumentNullException(nameof(filter));

        Filters.Add(filter);
    }

    /// <summary>
    /// Removes a log filter from the configuration.
    /// </summary>
    /// <param name="filter">The filter to remove.</param>
    /// <exception cref="ArgumentNullException">Thrown when filter is null.</exception>
    public void RemoveFilter(ILogFilter filter)
    {
        if (filter == null)
            throw new ArgumentNullException(nameof(filter));

        Filters.Remove(filter);
    }

    /// <summary>
    /// Sets the minimum log level for all categories.
    /// </summary>
    /// <param name="level">The minimum log level.</param>
    public void SetMinimumLevel(LogLevel level)
    {
        MinimumLevel = level;
    }

    /// <summary>
    /// Sets the minimum log level for a specific category.
    /// </summary>
    /// <param name="category">The category name.</param>
    /// <param name="level">The minimum log level for the category.</param>
    /// <exception cref="ArgumentException">Thrown when category is null or empty.</exception>
    public void SetMinimumLevel(string category, LogLevel level)
    {
        if (string.IsNullOrEmpty(category))
            throw new ArgumentException("Category cannot be null or empty", nameof(category));

        _categoryLevels[category] = level;
    }

    /// <summary>
    /// Gets the minimum log level for a specific category.
    /// </summary>
    /// <param name="category">The category name.</param>
    /// <returns>The minimum log level for the category, or the global minimum level if not set.</returns>
    public LogLevel GetMinimumLevel(string category)
    {
        if (string.IsNullOrEmpty(category))
            return MinimumLevel;

        return _categoryLevels.TryGetValue(category, out var level) ? level : MinimumLevel;
    }
}
