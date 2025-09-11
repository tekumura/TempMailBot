// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using TempMailBot.Logging.Core;
using TempMailBot.Logging.Enums;

namespace TempMailBot.Logging.Extensions;

/// <summary>
/// Extension methods that simplify structured and contextual logging.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Logs a message with optional contextual properties.
    /// </summary>
    public static void LogWithContext(this ILogger logger, LogLevel level, string message, IDictionary<string, object>? context = null)
    {
        if (context != null && context.Any())
        {
            using var scope = logger.BeginScope(context);
            logger.Log(level, message);
        }
        else
        {
            logger.Log(level, message);
        }
    }

    /// <summary>
    /// Logs a message and exception with optional contextual properties.
    /// </summary>
    public static void LogWithContext(this ILogger logger, LogLevel level, string message, Exception? exception, IDictionary<string, object>? context = null)
    {
        if (context != null && context.Any())
        {
            using var scope = logger.BeginScope(context);
            logger.Log(level, message, exception);
        }
        else
        {
            logger.Log(level, message, exception);
        }
    }

    /// <summary>
    /// Logs a performance entry with a standard set of properties.
    /// </summary>
    public static void LogPerformance(this ILogger logger, string operation, TimeSpan duration, IDictionary<string, object>? additionalData = null)
    {
        var context = new Dictionary<string, object>
        {
            ["Operation"] = operation,
            ["Duration"] = duration.TotalMilliseconds,
            ["DurationFormatted"] = duration.ToString(@"hh\:mm\:ss\.fff")
        };

        if (additionalData != null)
        {
            foreach (var item in additionalData)
            {
                context[item.Key] = item.Value;
            }
        }

        logger.LogWithContext(LogLevel.Info, $"Performance: {operation} completed in {duration.TotalMilliseconds:F2}ms", context);
    }

    /// <summary>
    /// Logs an error with exception.
    /// </summary>
    public static void LogError(this ILogger logger, Exception exception, string message, IDictionary<string, object>? context = null)
    {
        logger.LogWithContext(LogLevel.Error, message, exception, context);
    }

    /// <summary>Logs a warning.</summary>
    public static void LogWarning(this ILogger logger, string message, IDictionary<string, object>? context = null)
    {
        logger.LogWithContext(LogLevel.Warning, message, context);
    }

    /// <summary>Logs informational message.</summary>
    public static void LogInformation(this ILogger logger, string message, IDictionary<string, object>? context = null)
    {
        logger.LogWithContext(LogLevel.Info, message, context);
    }

    /// <summary>Logs debug message.</summary>
    public static void LogDebug(this ILogger logger, string message, IDictionary<string, object>? context = null)
    {
        logger.LogWithContext(LogLevel.Debug, message, context);
    }

    /// <summary>Logs trace message.</summary>
    public static void LogTrace(this ILogger logger, string message, IDictionary<string, object>? context = null)
    {
        logger.LogWithContext(LogLevel.Trace, message, context);
    }

    /// <summary>Logs critical message (no exception).</summary>
    public static void LogCritical(this ILogger logger, string message, IDictionary<string, object>? context = null)
    {
        logger.LogWithContext(LogLevel.Critical, message, context);
    }

    /// <summary>Logs critical message with exception.</summary>
    public static void LogCritical(this ILogger logger, Exception exception, string message, IDictionary<string, object>? context = null)
    {
        logger.LogWithContext(LogLevel.Critical, message, exception, context);
    }

    /// <summary>Begins a structured logging scope from a dictionary.</summary>
    public static IDisposable BeginScope(this ILogger logger, IDictionary<string, object> context)
    {
        return logger.BeginScope<IDictionary<string, object>>(context);
    }

    /// <summary>Begins a structured logging scope with a single key/value.</summary>
    public static IDisposable BeginScope(this ILogger logger, string key, object value)
    {
        var context = new Dictionary<string, object> { [key] = value };
        return logger.BeginScope(context);
    }

    /// <summary>Begins a structured logging scope from key/value tuples.</summary>
    public static IDisposable BeginScope(this ILogger logger, params (string key, object value)[] properties)
    {
        var context = properties.ToDictionary(p => p.key, p => p.value);
        return logger.BeginScope(context);
    }
}
