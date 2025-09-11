// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TempMailBot.Logging.Core;
using TempMailBot.Telegram.Core;
using TempMailBot.Telegram.Handlers;
using TempMailBot.Telegram.Processors.Commands;
using TempMailBot.Telegram.Processors.InlineButtons;

namespace TempMailBot.Telegram.Extensions;

/// <summary>
/// Dependency injection extensions for configuring the Telegram bot.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Telegram bot services and dependencies.
    /// </summary>
    public static IServiceCollection AddTelegramBot(this IServiceCollection services, Action<TelegramOptions> configure)
    {
        services.AddOptions<TelegramOptions>().Configure(configure).Validate(o => !string.IsNullOrWhiteSpace(o.Token), "Telegram token is required");

        services.TryAddSingleton<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<TelegramOptions>>().Value;
            return new TelegramBotClient(options.Token);
        });

        services.TryAddSingleton<ILogger>(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("Telegram"));

        services.AddSingleton<CommandProcessor>();
        services.AddSingleton<InlineButtonProcessor>(sp => new InlineButtonProcessor(
            sp.GetRequiredService<ITelegramBotClient>(),
            sp.GetRequiredService<ILogger>()));

        services.AddSingleton<IUpdateHandler>(sp => new MessageUpdateHandler(
            sp.GetRequiredService<CommandProcessor>(),
            sp.GetRequiredService<ILogger>()));

        services.AddSingleton<IUpdateHandler>(sp => new CallbackQueryUpdateHandler(
            sp.GetRequiredService<InlineButtonProcessor>(),
            sp.GetRequiredService<ILogger>()));

        services.AddSingleton<TelegramBot>(sp => new TelegramBot(
            sp.GetRequiredService<ITelegramBotClient>(),
            sp.GetRequiredService<CommandProcessor>(),
            sp.GetRequiredService<InlineButtonProcessor>(),
            sp.GetServices<IUpdateHandler>(),
            sp.GetRequiredService<ILogger>()));

        return services;
    }
}


