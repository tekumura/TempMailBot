// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using TempMailBot.Logging.Enums;

namespace TempMailBot.Logging.Configuration;

/// <summary>
/// High-level configuration options for the logging subsystem.
/// </summary>
public class LoggingOptions
{
    public LogLevel MinimumLevel { get; set; } = LogLevel.Info;
    public Dictionary<string, LogLevel> CategoryLevels { get; set; } = new();
    public List<HandlerOptions> Handlers { get; set; } = new();
    public List<FilterOptions> Filters { get; set; } = new();
    public bool EnableScopes { get; set; } = true;
    public bool EnableStructuredLogging { get; set; } = false;
    public string DefaultFormatter { get; set; } = "Default";
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Base handler options entry used for dynamic configuration.
/// </summary>
public class HandlerOptions
{
    public string Type { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public string Formatter { get; set; } = "Default";
    public Dictionary<string, object> Settings { get; set; } = new();
}

/// <summary>
/// Base filter options entry used for dynamic configuration.
/// </summary>
public class FilterOptions
{
    public string Type { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public Dictionary<string, object> Settings { get; set; } = new();
}
