// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using TempMailBot.Logging.Core;

namespace TempMailBot.Logging.Implementation;

/// <summary>
/// Provides a factory for creating logger instances with configuration and provider support.
/// </summary>
/// <remarks>
/// This factory manages logger instances using a thread-safe cache and supports
/// multiple logger providers for extensibility.
/// </remarks>
public class LoggerFactory : ILoggerFactory
{
    private readonly ILoggerConfiguration _configuration;
    private readonly ConcurrentDictionary<string, ILogger> _loggers;
    private readonly ConcurrentBag<ILoggerProvider> _providers;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerFactory"/> class.
    /// </summary>
    /// <param name="configuration">The logger configuration.</param>
    /// <exception cref="ArgumentNullException">Thrown when configuration is null.</exception>
    public LoggerFactory(ILoggerConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _loggers = new ConcurrentDictionary<string, ILogger>();
        _providers = new ConcurrentBag<ILoggerProvider>();
    }

    /// <summary>
    /// Creates a logger with the specified name.
    /// </summary>
    /// <param name="name">The name of the logger.</param>
    /// <returns>A logger instance.</returns>
    /// <exception cref="ArgumentException">Thrown when name is null or empty.</exception>
    public ILogger CreateLogger(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Logger name cannot be null or empty", nameof(name));

        return _loggers.GetOrAdd(name, CreateLoggerInternal);
    }

    /// <summary>
    /// Creates a logger for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to create a logger for.</typeparam>
    /// <returns>A logger instance.</returns>
    public ILogger CreateLogger<T>()
    {
        return CreateLogger(typeof(T).FullName ?? typeof(T).Name);
    }

    /// <summary>
    /// Adds a logger provider to the factory.
    /// </summary>
    /// <param name="provider">The provider to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when provider is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the factory is disposed.</exception>
    public void AddProvider(ILoggerProvider provider)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        if (_disposed)
            throw new ObjectDisposedException(nameof(LoggerFactory));

        _providers.Add(provider);
    }

    /// <summary>
    /// Removes a logger provider from the factory.
    /// </summary>
    /// <param name="provider">The provider to remove.</param>
    /// <exception cref="NotImplementedException">Provider removal is not supported in this implementation.</exception>
    public void RemoveProvider(ILoggerProvider provider)
    {
        throw new NotImplementedException("Provider removal is not supported in this implementation");
    }

    private ILogger CreateLoggerInternal(string name)
    {
        var logger = new Logger(name, _configuration);
        
        foreach (var provider in _providers)
        {
            try
            {
                var providerLogger = provider.CreateLogger(name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating logger from provider: {ex.Message}");
            }
        }

        return logger;
    }

    /// <summary>
    /// Disposes the logger factory and all its providers.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            foreach (var provider in _providers)
            {
                try
                {
                    provider.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error disposing provider: {ex.Message}");
                }
            }

            _disposed = true;
        }
    }
}
