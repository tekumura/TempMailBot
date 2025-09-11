// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TempMailBot.Logging.Configuration;
using TempMailBot.Logging.Core;
using TempMailBot.Logging.Enums;
using TempMailBot.Logging.Filters;
using TempMailBot.Logging.Formatters;
using TempMailBot.Logging.Handlers;
using TempMailBot.Logging.Implementation;

namespace TempMailBot.Logging.Extensions;

/// <summary>
/// Dependency injection extensions to configure the custom logging subsystem.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds logging services with optional inline configuration.
    /// </summary>
    public static IServiceCollection AddMuraLogging(this IServiceCollection services, Action<LoggingOptions>? configure = null)
    {
        var options = new LoggingOptions();
        configure?.Invoke(options);

        // Register formatters first (used by handlers)
        services.TryAddSingleton<ILogFormatter, DefaultLogFormatter>();
        services.TryAddSingleton<ILogFormatter, JsonLogFormatter>();
        services.TryAddSingleton<ILogFormatter, StructuredLogFormatter>();

        // Build and register a configured LoggerConfiguration
        services.TryAddSingleton<ILoggerConfiguration>(sp => BuildConfiguration(sp, options));

        // Logger factory and default logger
        services.TryAddSingleton<ILoggerFactory, LoggerFactory>();
        services.TryAddScoped<ILogger>(provider => provider.GetRequiredService<ILoggerFactory>().CreateLogger("Default"));

        return services;
    }

    /// <summary>
    /// Adds logging services with a provided <see cref="LoggingOptions"/> instance.
    /// </summary>
    public static IServiceCollection AddMuraLogging(this IServiceCollection services, LoggingOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        // Register formatters first (used by handlers)
        services.TryAddSingleton<ILogFormatter, DefaultLogFormatter>();
        services.TryAddSingleton<ILogFormatter, JsonLogFormatter>();
        services.TryAddSingleton<ILogFormatter, StructuredLogFormatter>();

        // Build and register a configured LoggerConfiguration
        services.TryAddSingleton<ILoggerConfiguration>(sp => BuildConfiguration(sp, options));

        // Logger factory and default logger
        services.TryAddSingleton<ILoggerFactory, LoggerFactory>();
        services.TryAddScoped<ILogger>(provider => provider.GetRequiredService<ILoggerFactory>().CreateLogger("Default"));

        return services;
    }

    /// <summary>
    /// Builds a configured <see cref="ILoggerConfiguration"/> instance from options.
    /// </summary>
    private static ILoggerConfiguration BuildConfiguration(IServiceProvider services, LoggingOptions options)
    {
        var config = new LoggerConfiguration();

        // Minimum level and category levels
        config.SetMinimumLevel(options.MinimumLevel);
        foreach (var categoryLevel in options.CategoryLevels)
        {
            config.SetMinimumLevel(categoryLevel.Key, categoryLevel.Value);
        }

        // Handlers
        foreach (var handlerOption in options.Handlers.Where(h => h.Enabled))
        {
            var handler = CreateHandler(services, handlerOption);
            if (handler != null)
            {
                config.AddHandler(handler);
            }
        }

        // Filters
        foreach (var filterOption in options.Filters.Where(f => f.Enabled))
        {
            var filter = CreateFilter(services, filterOption);
            if (filter != null)
            {
                config.AddFilter(filter);
            }
        }

        return config;
    }

    /// <summary>
    /// Registers handler-specific options from dynamic settings.
    /// </summary>
    private static void ConfigureHandler(IServiceCollection services, HandlerOptions handlerOption)
    {
        switch (handlerOption.Type.ToLowerInvariant())
        {
            case "console":
                services.Configure<ConsoleHandlerOptions>(opt =>
                {
                    if (TryGetBool(handlerOption.Settings, "UseColors", out var useColors))
                        opt.UseColors = useColors;
                });
                break;

            case "file":
                services.Configure<FileHandlerOptions>(opt =>
                {
                    if (TryGetString(handlerOption.Settings, "LogDirectory", out var dir))
                        opt.LogDirectory = dir;
                    if (TryGetInt(handlerOption.Settings, "MaxFileSize", out var max))
                        opt.MaxFileSize = max;
                    if (TryGetInt(handlerOption.Settings, "MaxBackupCount", out var backups))
                        opt.MaxBackupCount = backups;
                });
                break;

            case "database":
                services.Configure<DatabaseHandlerOptions>(opt =>
                {
                    if (TryGetString(handlerOption.Settings, "ConnectionString", out var cs))
                        opt.ConnectionString = cs;
                    if (TryGetString(handlerOption.Settings, "TableName", out var tn))
                        opt.TableName = tn;
                });
                break;
        }
    }

    /// <summary>
    /// Registers filter-specific options from dynamic settings.
    /// </summary>
    private static void ConfigureFilter(IServiceCollection services, FilterOptions filterOption)
    {
        switch (filterOption.Type.ToLowerInvariant())
        {
            case "level":
                if (TryGetString(filterOption.Settings, "MinLevel", out var levelStr) &&
                    Enum.TryParse<LogLevel>(levelStr, out var level))
                {
                    services.Configure<LoggerConfiguration>(config =>
                    {
                        config.AddFilter(new LevelFilter(level));
                    });
                }
                break;

            case "time":
                if (TryGetString(filterOption.Settings, "StartTime", out var startStr) &&
                    TryGetString(filterOption.Settings, "EndTime", out var endStr) &&
                    TimeSpan.TryParse(startStr, out var startTime) &&
                    TimeSpan.TryParse(endStr, out var endTime))
                {
                    services.Configure<LoggerConfiguration>(config =>
                    {
                        config.AddFilter(new TimeFilter(startTime, endTime));
                    });
                }
                break;

            case "category":
                if (TryGetString(filterOption.Settings, "Pattern", out var pattern))
                {
                    var isInclusive = TryGetBool(filterOption.Settings, "IsInclusive", out var incl) && incl;
                    
                    services.Configure<LoggerConfiguration>(config =>
                    {
                        config.AddFilter(new CategoryFilter(pattern, isInclusive));
                    });
                }
                break;
        }
    }

    /// <summary>
    /// Creates a concrete <see cref="ILogHandler"/> instance from dynamic handler options.
    /// </summary>
    /// <param name="services">The service provider used to resolve dependencies like formatters.</param>
    /// <param name="handlerOption">The handler option describing the handler type and settings.</param>
    /// <returns>An <see cref="ILogHandler"/> instance or null if options are incomplete/disabled.</returns>
    private static ILogHandler? CreateHandler(IServiceProvider services, HandlerOptions handlerOption)
    {
        switch (handlerOption.Type.ToLowerInvariant())
        {
            case "console":
                return new ConsoleLogHandler(services.GetRequiredService<ILogFormatter>());
            case "file":
                // Bind options to ensure directory and sizes are respected
                var fileOptions = new FileHandlerOptions();
                if (TryGetString(handlerOption.Settings, "LogDirectory", out var dir)) fileOptions.LogDirectory = dir;
                if (TryGetInt(handlerOption.Settings, "MaxFileSize", out var max)) fileOptions.MaxFileSize = max;
                if (TryGetInt(handlerOption.Settings, "MaxBackupCount", out var backups)) fileOptions.MaxBackupCount = backups;

                var formatter = services.GetRequiredService<ILogFormatter>();
                return new FileLogHandler(fileOptions.LogDirectory, formatter, fileOptions.MaxFileSize, fileOptions.MaxBackupCount);
            case "database":
                // optional, only if configured
                if (TryGetString(handlerOption.Settings, "ConnectionString", out var cs) &&
                    TryGetString(handlerOption.Settings, "TableName", out var tn))
                {
                    return new DatabaseLogHandler(cs, tn, services.GetRequiredService<ILogFormatter>());
                }
                return null;
        }
        return null;
    }

    /// <summary>
    /// Creates a concrete <see cref="ILogFilter"/> instance from dynamic filter options.
    /// </summary>
    /// <param name="services">The service provider (not used currently, reserved for future needs).</param>
    /// <param name="filterOption">The filter option describing the filter type and settings.</param>
    /// <returns>An <see cref="ILogFilter"/> instance or null if options are invalid.</returns>
    private static ILogFilter? CreateFilter(IServiceProvider services, FilterOptions filterOption)
    {
        switch (filterOption.Type.ToLowerInvariant())
        {
            case "level":
                if (TryGetString(filterOption.Settings, "MinLevel", out var levelStr) && Enum.TryParse<LogLevel>(levelStr, out var level))
                    return new LevelFilter(level);
                break;
            case "time":
                if (TryGetString(filterOption.Settings, "StartTime", out var startStr) &&
                    TryGetString(filterOption.Settings, "EndTime", out var endStr) &&
                    TimeSpan.TryParse(startStr, out var startTime) &&
                    TimeSpan.TryParse(endStr, out var endTime))
                    return new TimeFilter(startTime, endTime);
                break;
            case "category":
                if (TryGetString(filterOption.Settings, "Pattern", out var pattern))
                {
                    var isInclusive = TryGetBool(filterOption.Settings, "IsInclusive", out var incl) && incl;
                    return new CategoryFilter(pattern, isInclusive);
                }
                break;
        }
        return null;
    }

    // Helpers to read loose-typed configuration values safely
    private static bool TryGetString(IDictionary<string, object> settings, string key, out string value)
    {
        value = string.Empty;
        if (!settings.ContainsKey(key) || settings[key] is null) return false;
        var obj = settings[key];
        switch (obj)
        {
            case string s:
                value = s; return true;
            case JsonElement el when el.ValueKind == JsonValueKind.String:
                value = el.GetString() ?? string.Empty; return !string.IsNullOrEmpty(value);
            default:
                value = obj.ToString() ?? string.Empty; return !string.IsNullOrEmpty(value);
        }
    }

    private static bool TryGetInt(IDictionary<string, object> settings, string key, out int value)
    {
        value = 0;
        if (!settings.ContainsKey(key) || settings[key] is null) return false;
        var obj = settings[key];
        switch (obj)
        {
            case int i:
                value = i; return true;
            case long l:
                value = (int)l; return true;
            case double d:
                value = (int)d; return true;
            case string s when int.TryParse(s, out var parsed):
                value = parsed; return true;
            case JsonElement el when el.ValueKind == JsonValueKind.Number:
                if (el.TryGetInt32(out var i32)) { value = i32; return true; }
                if (el.TryGetInt64(out var i64)) { value = (int)i64; return true; }
                break;
        }
        return false;
    }

    private static bool TryGetBool(IDictionary<string, object> settings, string key, out bool value)
    {
        value = false;
        if (!settings.ContainsKey(key) || settings[key] is null) return false;
        var obj = settings[key];
        switch (obj)
        {
            case bool b:
                value = b; return true;
            case string s when bool.TryParse(s, out var parsed):
                value = parsed; return true;
            case JsonElement el when el.ValueKind == JsonValueKind.True || el.ValueKind == JsonValueKind.False:
                value = el.GetBoolean(); return true;
        }
        return false;
    }
}
