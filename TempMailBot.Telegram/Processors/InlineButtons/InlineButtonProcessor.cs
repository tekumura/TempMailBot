// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Telegram.Bot;
using Telegram.Bot.Types;
using TempMailBot.Logging.Core;

namespace TempMailBot.Telegram.Processors.InlineButtons;

/// <summary>
/// Handles the registration and execution of inline button callbacks for the Telegram bot.
/// </summary>
/// <remarks>
/// This processor manages a collection of inline button handlers and routes callback queries
/// to the appropriate handlers based on the callback data.
/// </remarks>
public class InlineButtonProcessor
{
    private readonly ITelegramBotClient _botClient;
    private readonly Dictionary<string, IInlineButton> _inlineButtons = new();
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineButtonProcessor"/> class.
    /// </summary>
    /// <param name="botClient">The Telegram bot client.</param>
    /// <param name="logger">The logger instance.</param>
    public InlineButtonProcessor(ITelegramBotClient botClient, ILogger logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    /// <summary>
    /// Registers an inline button handler.
    /// </summary>
    /// <param name="button">The inline button handler to register.</param>
    public void RegisterInlineButton(IInlineButton button)
    {
        _inlineButtons[button.CallbackData] = button;
        _logger.LogInformation("Registered inline button: {0}", button.CallbackData);
    }

    /// <summary>
    /// Handles an inline button callback query.
    /// </summary>
    /// <param name="callbackQuery">The callback query to handle.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleInlineButtonAsync(CallbackQuery callbackQuery)
    {
        if (_inlineButtons.TryGetValue(callbackQuery.Data!, out var button))
        {
            _logger.LogInformation("Executing inline button {0} in chat {1}", callbackQuery.Data, callbackQuery.Message!.Chat.Id);
            await button.OnClickAsync(_botClient, callbackQuery);
        }
        else
        {
            _logger.LogWarning("Unknown inline button: {0}", callbackQuery.Data);
        }
    }
}