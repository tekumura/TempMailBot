// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Telegram.Bot;
using Telegram.Bot.Types;

namespace TempMailBot.Telegram.Processors.InlineButtons;

/// <summary>
/// Defines a contract for inline buttons with callback handling.
/// </summary>
public interface IInlineButton
{
    /// <summary>
    /// Static prefix of callback data this button handles.
    /// </summary>
    string CallbackData { get; }
    /// <summary>
    /// Handles a callback query that matches <see cref="CallbackData"/>.
    /// </summary>
    Task OnClickAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery);
}