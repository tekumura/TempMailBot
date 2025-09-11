// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TempMailBot.Telegram.Processors.InlineButtons;
using TempMailBot.Client.Data;

namespace TempMailBot.Client.Telegram.Buttons;

/// <summary>
/// Inline button to delete a specific temporary email address.
/// </summary>
public class DeleteEmailButton : IInlineButton
{
    public string CallbackData => "delete_email_by_address";

    private readonly SqliteRepository _repo;

    /// <summary>
    /// Initializes a new instance of <see cref="DeleteEmailButton"/>.
    /// </summary>
    public DeleteEmailButton(SqliteRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Deletes the email and updates the message.
    /// </summary>
    public async Task OnClickAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        var userId = callbackQuery.From.Id;
        var parts = callbackQuery.Data!.Split(':', 2);
        if (parts.Length != 2)
        {
            await botClient.AnswerCallbackQuery(callbackQuery.Id, "Некорректные данные кнопки");
            return;
        }
        var address = parts[1];
        _repo.DeleteEmailByAddress(userId, address);
        await botClient.EditMessageText(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId, $"✅ Email <code>{address}</code> удален", parseMode: ParseMode.Html);
    }
}


