// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;

namespace TempMailBot.Logging.Models;

/// <summary>
/// Represents a context for structured logging that can hold properties and scopes.
/// </summary>
/// <remarks>
/// This class provides a way to add contextual information to log entries and manage
/// scoped logging contexts. It implements IDisposable to automatically clean up
/// when the context goes out of scope.
/// </remarks>
public class LogContext : IDisposable
{
    private static readonly AsyncLocal<LogContext?> _current = new();
    private readonly ConcurrentDictionary<string, object> _properties;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogContext"/> class.
    /// </summary>
    public LogContext()
    {
        _properties = new ConcurrentDictionary<string, object>();
    }

    /// <summary>
    /// Gets or sets the current log context for the current async context.
    /// </summary>
    /// <value>The current log context, or null if no context is set.</value>
    public static LogContext? Current
    {
        get => _current.Value;
        set => _current.Value = value;
    }

    /// <summary>
    /// Gets the properties associated with this log context.
    /// </summary>
    /// <value>A read-only dictionary of properties.</value>
    public IReadOnlyDictionary<string, object> Properties => _properties;

    /// <summary>
    /// Sets a property in the log context.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The property value.</param>
    /// <exception cref="ArgumentException">Thrown when key is null or empty.</exception>
    public void SetProperty(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        _properties[key] = value;
    }

    /// <summary>
    /// Gets a property from the log context with the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <returns>The property value, or default(T) if not found or type mismatch.</returns>
    /// <exception cref="ArgumentException">Thrown when key is null or empty.</exception>
    public T? GetProperty<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        return _properties.TryGetValue(key, out var value) && value is T typedValue ? typedValue : default;
    }

    /// <summary>
    /// Tries to get a property from the log context with the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="value">When this method returns, contains the property value if found and type matches; otherwise, default(T).</param>
    /// <returns>True if the property was found and type matches; otherwise, false.</returns>
    public bool TryGetProperty<T>(string key, out T? value)
    {
        value = default;
        
        if (string.IsNullOrEmpty(key))
            return false;

        if (_properties.TryGetValue(key, out var objValue) && objValue is T typedValue)
        {
            value = typedValue;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes a property from the log context.
    /// </summary>
    /// <param name="key">The property key to remove.</param>
    public void RemoveProperty(string key)
    {
        if (!string.IsNullOrEmpty(key))
        {
            _properties.TryRemove(key, out _);
        }
    }

    /// <summary>
    /// Clears all properties from the log context.
    /// </summary>
    public void Clear()
    {
        _properties.Clear();
    }

    /// <summary>
    /// Begins a new log scope.
    /// </summary>
    /// <returns>A new log context that becomes the current context.</returns>
    public static LogContext BeginScope()
    {
        var context = new LogContext();
        Current = context;
        return context;
    }

    /// <summary>
    /// Begins a new log scope with an initial property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The property value.</param>
    /// <returns>A new log context that becomes the current context.</returns>
    public static LogContext BeginScope(string key, object value)
    {
        var context = BeginScope();
        context.SetProperty(key, value);
        return context;
    }

    /// <summary>
    /// Disposes the log context and cleans up resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            if (Current == this)
            {
                Current = null;
            }
            
            _disposed = true;
        }
    }
}
