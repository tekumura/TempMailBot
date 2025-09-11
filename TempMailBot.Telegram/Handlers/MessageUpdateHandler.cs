// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TempMailBot.Telegram.Processors.Commands;
using TempMailBot.Logging.Core;

namespace TempMailBot.Telegram.Handlers;

/// <summary>
/// Handles text message updates and forwards commands to <see cref="CommandProcessor"/>.
/// </summary>
public class MessageUpdateHandler : IUpdateHandler
{
    private readonly CommandProcessor _commandProcessor;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new instance of <see cref="MessageUpdateHandler"/>.
    /// </summary>
    public MessageUpdateHandler(CommandProcessor commandProcessor, ILogger logger)
    {
        _commandProcessor  = commandProcessor;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update is not { Type: UpdateType.Message, Message.Text: not null }) 
            return false;
        
        _logger.LogDebug("Handling message from chat {0}", update.Message!.Chat.Id);
        await _commandProcessor.HandleCommandAsync(update.Message);
        
        return true;
    }
}