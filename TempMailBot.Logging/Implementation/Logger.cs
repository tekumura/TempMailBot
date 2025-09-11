// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using TempMailBot.Logging.Core;
using TempMailBot.Logging.Enums;
using TempMailBot.Logging.Filters;
using TempMailBot.Logging.Handlers;
using TempMailBot.Logging.Models;

namespace TempMailBot.Logging.Implementation;

/// <summary>
/// Provides a concrete implementation of the <see cref="ILogger"/> interface for logging messages.
/// </summary>
/// <remarks>
/// This logger supports multiple handlers, filters, and scoped logging. It uses thread-safe collections
/// for concurrent access and provides various logging methods for different log levels.
/// </remarks>
public class Logger : ILogger
{
    private readonly string _name;
    private readonly ILoggerConfiguration _configuration;
    private readonly ConcurrentBag<ILogHandler> _handlers;
    private readonly ConcurrentBag<ILogFilter> _filters;
    private readonly ConcurrentDictionary<string, object> _scopeData;

    /// <summary>
    /// Initializes a new instance of the <see cref="Logger"/> class.
    /// </summary>
    /// <param name="name">The name of the logger.</param>
    /// <param name="configuration">The logger configuration.</param>
    /// <exception cref="ArgumentNullException">Thrown when name or configuration is null.</exception>
    public Logger(string name, ILoggerConfiguration configuration)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _handlers = new ConcurrentBag<ILogHandler>();
        _filters = new ConcurrentBag<ILogFilter>();
        _scopeData = new ConcurrentDictionary<string, object>();
        
        // Add default handlers from configuration
        foreach (var handler in _configuration.Handlers)
        {
            _handlers.Add(handler);
        }
        
        // Add default filters from configuration
        foreach (var filter in _configuration.Filters)
        {
            _filters.Add(filter);
        }
    }

    /// <summary>
    /// Gets the name of the logger.
    /// </summary>
    /// <value>The logger name.</value>
    public string Name => _name;

    /// <summary>
    /// Logs a message with the specified level.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The message to log.</param>
    public void Log(LogLevel level, string message)
    {
        Log(level, message, (Exception?)null);
    }

    /// <summary>
    /// Logs a message with the specified level and exception.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Log(LogLevel level, string message, Exception? exception)
    {
        if (!IsEnabled(level))
            return;

        var logEntry = new LogEntry(_name, level, message, exception, DateTime.UtcNow, _scopeData.ToDictionary(x => x.Key, x => x.Value));
        
        if (ShouldLog(logEntry))
        {
            foreach (var handler in _handlers)
            {
                try
                {
                    handler.Handle(logEntry);
                }
                catch (Exception ex)
                {
                    // Log the error to console to avoid infinite loops
                    Console.WriteLine($"Error in log handler: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Logs a formatted message with the specified level.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void Log(LogLevel level, string message, params object[] args)
    {
        Log(level, string.Format(message, args));
    }

    /// <summary>
    /// Logs a formatted message with the specified level and exception.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void Log(LogLevel level, Exception? exception, string message, params object[] args)
    {
        Log(level, string.Format(message, args), exception);
    }

    /// <summary>
    /// Logs a trace message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogTrace(string message)
    {
        Log(LogLevel.Trace, message);
    }

    /// <summary>
    /// Logs a formatted trace message.
    /// </summary>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void LogTrace(string message, params object[] args)
    {
        Log(LogLevel.Trace, message, args);
    }

    /// <summary>
    /// Logs a formatted trace message with exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void LogTrace(Exception? exception, string message, params object[] args)
    {
        Log(LogLevel.Trace, exception, message, args);
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogDebug(string message)
    {
        Log(LogLevel.Debug, message);
    }

    /// <summary>
    /// Logs a formatted debug message.
    /// </summary>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void LogDebug(string message, params object[] args)
    {
        Log(LogLevel.Debug, message, args);
    }

    /// <summary>
    /// Logs a formatted debug message with exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void LogDebug(Exception? exception, string message, params object[] args)
    {
        Log(LogLevel.Debug, exception, message, args);
    }

    /// <summary>
    /// Logs an information message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogInformation(string message)
    {
        Log(LogLevel.Info, message);
    }

    /// <summary>
    /// Logs a formatted information message.
    /// </summary>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void LogInformation(string message, params object[] args)
    {
        Log(LogLevel.Info, message, args);
    }

    /// <summary>
    /// Logs a formatted information message with exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void LogInformation(Exception? exception, string message, params object[] args)
    {
        Log(LogLevel.Info, exception, message, args);
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogWarning(string message)
    {
        Log(LogLevel.Warning, message);
    }

    /// <summary>
    /// Logs a formatted warning message.
    /// </summary>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void LogWarning(string message, params object[] args)
    {
        Log(LogLevel.Warning, message, args);
    }

    /// <summary>
    /// Logs a formatted warning message with exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void LogWarning(Exception? exception, string message, params object[] args)
    {
        Log(LogLevel.Warning, exception, message, args);
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogError(string message)
    {
        Log(LogLevel.Error, message);
    }

    /// <summary>
    /// Logs a formatted error message.
    /// </summary>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void LogError(string message, params object[] args)
    {
        Log(LogLevel.Error, message, args);
    }

    /// <summary>
    /// Logs a formatted error message with exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void LogError(Exception? exception, string message, params object[] args)
    {
        Log(LogLevel.Error, exception, message, args);
    }

    /// <summary>
    /// Logs a critical message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogCritical(string message)
    {
        Log(LogLevel.Critical, message);
    }

    /// <summary>
    /// Logs a formatted critical message.
    /// </summary>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void LogCritical(string message, params object[] args)
    {
        Log(LogLevel.Critical, message, args);
    }

    /// <summary>
    /// Logs a formatted critical message with exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message format string.</param>
    /// <param name="args">The arguments to format the message.</param>
    public void LogCritical(Exception? exception, string message, params object[] args)
    {
        Log(LogLevel.Critical, exception, message, args);
    }

    /// <summary>
    /// Adds a log handler to the logger.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    public void AddHandler(ILogHandler handler)
    {
        if (handler != null)
        {
            _handlers.Add(handler);
        }
    }

    /// <summary>
    /// Removes a log handler from the logger.
    /// </summary>
    /// <param name="handler">The handler to remove.</param>
    /// <exception cref="NotImplementedException">Handler removal is not supported in this implementation.</exception>
    public void RemoveHandler(ILogHandler handler)
    {
        throw new NotImplementedException("Handler removal is not supported in this implementation");
    }

    /// <summary>
    /// Adds a log filter to the logger.
    /// </summary>
    /// <param name="filter">The filter to add.</param>
    public void AddFilter(ILogFilter filter)
    {
        if (filter != null)
        {
            _filters.Add(filter);
        }
    }

    /// <summary>
    /// Removes a log filter from the logger.
    /// </summary>
    /// <param name="filter">The filter to remove.</param>
    /// <exception cref="NotImplementedException">Filter removal is not supported in this implementation.</exception>
    public void RemoveFilter(ILogFilter filter)
    {
        throw new NotImplementedException("Filter removal is not supported in this implementation");
    }

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <param name="message">The message for the scope.</param>
    /// <returns>A disposable scope object.</returns>
    public IDisposable BeginScope(string message)
    {
        return BeginScope<object>(message);
    }

    /// <summary>
    /// Begins a logical operation scope with state.
    /// </summary>
    /// <typeparam name="T">The type of the state.</typeparam>
    /// <param name="state">The state for the scope.</param>
    /// <returns>A disposable scope object.</returns>
    public IDisposable BeginScope<T>(T state)
    {
        var scopeId = Guid.NewGuid().ToString();
        _scopeData[scopeId] = state ?? new object();
        
        return new ScopeDisposable(() => _scopeData.TryRemove(scopeId, out _));
    }

    private bool IsEnabled(LogLevel level)
    {
        return level >= _configuration.MinimumLevel;
    }

    private bool ShouldLog(LogEntry logEntry)
    {
        return _filters.All(filter => filter.IsEnabled(logEntry));
    }

    /// <summary>
    /// A disposable scope implementation for managing logger scopes.
    /// </summary>
    private class ScopeDisposable : IDisposable
    {
        private readonly Action _disposeAction;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeDisposable"/> class.
        /// </summary>
        /// <param name="disposeAction">The action to execute when disposing.</param>
        public ScopeDisposable(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        /// <summary>
        /// Disposes the scope and executes the dispose action.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposeAction();
                _disposed = true;
            }
        }
    }
}
