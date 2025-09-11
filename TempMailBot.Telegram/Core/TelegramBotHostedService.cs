// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TempMailBot.Telegram.Processors.Commands;
using TempMailBot.Telegram.Processors.InlineButtons;

namespace TempMailBot.Telegram.Core;

/// <summary>
/// Hosted service that starts the Telegram bot and registers commands/buttons.
/// </summary>
public class TelegramBotHostedService : BackgroundService
{
    private readonly TelegramBot _bot;
    private readonly TelegramOptions _options;
    private readonly IEnumerable<ICommand> _commands;
    private readonly IEnumerable<IInlineButton> _inlineButtons;

    /// <summary>
    /// Creates a new instance of the hosted service.
    /// </summary>
    public TelegramBotHostedService(TelegramBot bot, IOptions<TelegramOptions> options, IEnumerable<ICommand> commands, IEnumerable<IInlineButton> inlineButtons)
    {
        _bot = bot;
        _options = options.Value;
        _commands = commands;
        _inlineButtons = inlineButtons;
    }

    /// <summary>
    /// Registers commands and buttons, then starts polling.
    /// </summary>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var cmd in _commands)
        {
            _bot.RegisterCommand(cmd);
        }

        foreach (var btn in _inlineButtons)
        {
            _bot.RegisterInlineButton(btn);
        }

        _bot.Start(stoppingToken);
        return Task.CompletedTask;
    }
}


