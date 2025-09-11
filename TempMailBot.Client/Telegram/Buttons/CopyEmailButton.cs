// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Telegram.Bot;
using Telegram.Bot.Types;
using TempMailBot.Telegram.Processors.InlineButtons;

namespace TempMailBot.Client.Telegram.Buttons;

/// <summary>
/// Inline button to show a copy-able email address.
/// </summary>
public class CopyEmailButton : IInlineButton
{
    public string CallbackData => "copy_email";

    /// <summary>
    /// Answers callback with the email address payload.
    /// </summary>
    public async Task OnClickAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        var parts = callbackQuery.Data!.Split(':', 2);
        if (parts.Length != 2)
        {
            await botClient.AnswerCallbackQuery(callbackQuery.Id, "Некорректные данные кнопки");
            return;
        }
        var address = parts[1];
        await botClient.AnswerCallbackQuery(callbackQuery.Id, $"Email скопирован: {address}", showAlert: true);
    }
}


