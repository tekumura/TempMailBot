// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TempMailBot.Telegram.Processors.InlineButtons;
using TempMailBot.Logging.Core;

namespace TempMailBot.Telegram.Handlers;

/// <summary>
/// Handles callback query updates and dispatches to inline button processor.
/// </summary>
public class CallbackQueryUpdateHandler : IUpdateHandler
{
    private readonly InlineButtonProcessor _inlineButtonProcessor;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new instance of <see cref="CallbackQueryUpdateHandler"/>.
    /// </summary>
    public CallbackQueryUpdateHandler(InlineButtonProcessor inlineButtonProcessor, ILogger logger)
    {
        _inlineButtonProcessor = inlineButtonProcessor;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update is { Type: UpdateType.CallbackQuery, CallbackQuery: not null })
        {
            _logger.LogDebug("Handling callback query from chat {0}", update.CallbackQuery!.Message!.Chat.Id);
            await _inlineButtonProcessor.HandleInlineButtonAsync(update.CallbackQuery);
            return true;
        }

        return false;
    }
}