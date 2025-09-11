// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TempMailBot.Telegram.Handlers;
using TempMailBot.Telegram.Processors.Commands;
using TempMailBot.Telegram.Processors.InlineButtons;
using IUpdateHandler = TempMailBot.Telegram.Handlers.IUpdateHandler;
using TempMailBot.Logging.Core;

namespace TempMailBot.Telegram.Core;

/// <summary>
/// Thin wrapper around ITelegramBotClient that wires handlers and starts receiving updates.
/// </summary>
public class TelegramBot
{
    private readonly ITelegramBotClient _botClient;
    private readonly CommandProcessor _commandProcessor;
    private readonly InlineButtonProcessor _inlineButtonProcessor;
    private readonly IUpdateHandler[] _updateHandlers;
    private readonly ILogger _logger;
    
    /// <summary>
    /// Constructs the bot with injected dependencies.
    /// </summary>
    public TelegramBot(ITelegramBotClient botClient, CommandProcessor commandProcessor, InlineButtonProcessor inlineButtonProcessor, IEnumerable<IUpdateHandler> updateHandlers, ILogger logger)
    {
        _botClient = botClient;
        _commandProcessor = commandProcessor;
        _inlineButtonProcessor = inlineButtonProcessor;
        _updateHandlers = updateHandlers.ToArray();
        _logger = logger;
    }

    /// <summary>
    /// Registers a command implementation.
    /// </summary>
    public void RegisterCommand(ICommand command)
    {
        _commandProcessor.RegisterCommand(command);
    }

    /// <summary>
    /// Registers an inline button implementation.
    /// </summary>
    public void RegisterInlineButton(IInlineButton button)
    {
        _inlineButtonProcessor.RegisterInlineButton(button);
    }

    /// <summary>
    /// Starts receiving updates using long polling.
    /// </summary>
    public void Start(CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message,
                UpdateType.CallbackQuery
            }
        };

        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );

        _logger.LogInformation("Telegram bot started receiving updates.");
    }

    /// <summary>
    /// Dispatches an update to the first handler that can process it.
    /// </summary>
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Received update of type {0}", update.Type);
        foreach (var handler in _updateHandlers)
        {
            if (await handler.HandleUpdateAsync(update, cancellationToken))
            {
                _logger.LogTrace("Update handled by {0}", handler.GetType().Name);
                break;
            }
        }
    }

    /// <summary>
    /// Logs polling errors without stopping the receiver.
    /// </summary>
    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Polling error occurred: {0}", exception.Message);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns the underlying Telegram client.
    /// </summary>
    public ITelegramBotClient GetTelegramBotClient()
    {
        return _botClient;
    }
}