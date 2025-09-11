// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using TempMailBot.Logging.Enums;
using TempMailBot.Logging.Filters;
using TempMailBot.Logging.Handlers;

namespace TempMailBot.Logging.Core;

/// <summary>
/// Represents a logger that can write log messages with different levels and scopes.
/// </summary>
/// <remarks>
/// This interface provides methods for logging messages at different levels (Trace, Debug, Info, Warning, Error, Critical)
/// and supports structured logging with scopes and context data.
/// </remarks>
public interface ILogger
{
    /// <summary>
    /// Gets the name of the logger.
    /// </summary>
    /// <value>The name of the logger.</value>
    string Name { get; }
    
    /// <summary>
    /// Writes a log message at the specified level.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The message to log.</param>
    void Log(LogLevel level, string message);
    
    /// <summary>
    /// Writes a log message at the specified level with an exception.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    void Log(LogLevel level, string message, Exception? exception);
    
    /// <summary>
    /// Writes a formatted log message at the specified level.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void Log(LogLevel level, string message, params object[] args);
    
    /// <summary>
    /// Writes a formatted log message at the specified level with an exception.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void Log(LogLevel level, Exception? exception, string message, params object[] args);
    
    /// <summary>
    /// Writes a trace level log message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogTrace(string message);
    
    /// <summary>
    /// Writes a formatted trace level log message.
    /// </summary>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void LogTrace(string message, params object[] args);
    
    /// <summary>
    /// Writes a formatted trace level log message with an exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void LogTrace(Exception? exception, string message, params object[] args);
    
    /// <summary>
    /// Writes a debug level log message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogDebug(string message);
    
    /// <summary>
    /// Writes a formatted debug level log message.
    /// </summary>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void LogDebug(string message, params object[] args);
    
    /// <summary>
    /// Writes a formatted debug level log message with an exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void LogDebug(Exception? exception, string message, params object[] args);
    
    /// <summary>
    /// Writes an information level log message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogInformation(string message);
    
    /// <summary>
    /// Writes a formatted information level log message.
    /// </summary>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void LogInformation(string message, params object[] args);
    
    /// <summary>
    /// Writes a formatted information level log message with an exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void LogInformation(Exception? exception, string message, params object[] args);
    
    /// <summary>
    /// Writes a warning level log message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogWarning(string message);
    
    /// <summary>
    /// Writes a formatted warning level log message.
    /// </summary>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void LogWarning(string message, params object[] args);
    
    /// <summary>
    /// Writes a formatted warning level log message with an exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void LogWarning(Exception? exception, string message, params object[] args);
    
    /// <summary>
    /// Writes an error level log message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogError(string message);
    
    /// <summary>
    /// Writes a formatted error level log message.
    /// </summary>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void LogError(string message, params object[] args);
    
    /// <summary>
    /// Writes a formatted error level log message with an exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void LogError(Exception? exception, string message, params object[] args);
    
    /// <summary>
    /// Writes a critical level log message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogCritical(string message);
    
    /// <summary>
    /// Writes a formatted critical level log message.
    /// </summary>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void LogCritical(string message, params object[] args);
    
    /// <summary>
    /// Writes a formatted critical level log message with an exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message template to log.</param>
    /// <param name="args">The arguments for the message template.</param>
    void LogCritical(Exception? exception, string message, params object[] args);
    
    /// <summary>
    /// Adds a log handler to the logger.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    void AddHandler(ILogHandler handler);
    
    /// <summary>
    /// Removes a log handler from the logger.
    /// </summary>
    /// <param name="handler">The handler to remove.</param>
    void RemoveHandler(ILogHandler handler);
    
    /// <summary>
    /// Adds a log filter to the logger.
    /// </summary>
    /// <param name="filter">The filter to add.</param>
    void AddFilter(ILogFilter filter);
    
    /// <summary>
    /// Removes a log filter from the logger.
    /// </summary>
    /// <param name="filter">The filter to remove.</param>
    void RemoveFilter(ILogFilter filter);
    
    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <param name="message">The message for the scope.</param>
    /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
    IDisposable BeginScope(string message);
    
    /// <summary>
    /// Begins a logical operation scope with a state object.
    /// </summary>
    /// <typeparam name="T">The type of the state object.</typeparam>
    /// <param name="state">The state object for the scope.</param>
    /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
    IDisposable BeginScope<T>(T state);
}
