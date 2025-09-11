// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TempMailBot.Client.Data;
using TempMailBot.Client.Services;
using TempMailBot.Client.Telegram.Buttons;
using TempMailBot.Client.Telegram.Commands;
using TempMailBot.Logging.Configuration;
using TempMailBot.Logging.Enums;
using TempMailBot.Logging.Extensions;
using TempMailBot.Telegram.Core;
using TempMailBot.Telegram.Extensions;
using TempMailBot.Telegram.Processors.Commands;
using TempMailBot.Telegram.Processors.InlineButtons;

namespace TempMailBot.Client;

/// <summary>
/// Classic program entry type with a static Main.
/// </summary>
internal static class Program
{
    private static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, cfg) =>
            {
                cfg.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((ctx, services) =>
            {
                // Bind logging from config with env override fallback
                services.AddMuraLogging(options =>
                {
                    var section = ctx.Configuration.GetSection("Logging");
                    var min = section["MinimumLevel"];
                    if (!string.IsNullOrWhiteSpace(min) && Enum.TryParse<LogLevel>(min, out var parsed))
                        options.MinimumLevel = parsed;
                    else
                        options.MinimumLevel = LogLevel.Debug;

                    var handlers = section.GetSection("Handlers").Get<List<HandlerOptions>>();
                    if (handlers != null && handlers.Count > 0)
                    {
                        foreach (var h in handlers)
                            options.Handlers.Add(h);
                    }
                    else
                    {
                        options.Handlers.Add(new HandlerOptions { Type = "console", Enabled = true });
                        options.Handlers.Add(new HandlerOptions
                        {
                            Type = "file",
                            Enabled = true,
                            Settings = new Dictionary<string, object>
                            {
                                { "LogDirectory", Path.Combine(AppContext.BaseDirectory, "logs") },
                                { "MaxFileSize", 5 * 1024 * 1024 },
                                { "BackupCount", 5 }
                            }
                        });
                    }
                });

                services.AddTelegramBot(options =>
                {
                    // Prefer env var, fallback to config
                    options.Token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ??
                                    ctx.Configuration["Telegram:Token"] ?? "YOUR_TOKEN_HERE";
                    options.DropPendingUpdates = !bool.TryParse(ctx.Configuration["Telegram:DropPendingUpdates"], out var drop) || drop;

                    // AllowedUpdates (deprecated)
                    // var allowed = ctx.Configuration.GetSection("Telegram:AllowedUpdates").Get<string[]>() ?? Array.Empty<string>();
                    // if (allowed.Length > 0)
                    // {
                    //     try
                    //     {
                    //         options.AllowedUpdates = allowed
                    //         .Select(s => Enum.Parse<UpdateType>(s, ignoreCase: true))
                    //         .ToArray();
                    //     }
                    //     catch { /* ignore parse errors, keep defaults */ }
                    // }

                    // LongPollingTimeoutSeconds (deprecated)
                    // var lpts = ctx.Configuration["Telegram:LongPollingTimeoutSeconds"];
                    // if (!string.IsNullOrWhiteSpace(lpts) && int.TryParse(lpts, out var seconds) && seconds > 0)
                    // {
                    //     options.LongPollingTimeout = TimeSpan.FromSeconds(seconds);
                    // }
                });

                // App services
                services.AddSingleton<SqliteRepository>();
                services.AddSingleton<MailTmService>();

                // Commands
                services.AddSingleton<ICommand, HelpCommand>();
                services.AddSingleton<ICommand, GetEmailCommand>();
                services.AddSingleton<ICommand, MyEmailsCommand>();

                // Inline buttons
                services.AddSingleton<IInlineButton, CheckMailButton>();
                services.AddSingleton<IInlineButton, DeleteEmailButton>();
                services.AddSingleton<IInlineButton, CopyEmailButton>();

                services.AddHostedService<TelegramBotHostedService>();
            })
            .Build();

        host.Run();
    }
}