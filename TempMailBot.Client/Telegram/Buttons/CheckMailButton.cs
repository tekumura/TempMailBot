// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TempMailBot.Telegram.Processors.InlineButtons;
using TempMailBot.Client.Data;
using TempMailBot.Client.Services;

namespace TempMailBot.Client.Telegram.Buttons;

/// <summary>
/// Inline button to check messages for a specific temporary email address.
/// </summary>
public class CheckMailButton : IInlineButton
{
    public string CallbackData => "check_mail_by_address";

    private readonly SqliteRepository _repo;
    private readonly MailTmService _mail;

    /// <summary>
    /// Initializes a new instance of <see cref="CheckMailButton"/>.
    /// </summary>
    public CheckMailButton(SqliteRepository repo, MailTmService mail)
    {
        _repo = repo;
        _mail = mail;
    }

    /// <summary>
    /// Handles callback by loading recent messages and editing message text.
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
        var details = _repo.GetEmailByAddress(userId, address);
        if (details == null)
        {
            await botClient.EditMessageText(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId, $"Email <code>{address}</code> не найден", parseMode: ParseMode.Html);
            return;
        }

        await botClient.EditMessageText(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId, $"🔍 Загружаю письма для <code>{address}</code>...", parseMode: ParseMode.Html);

        var messages = await _mail.GetMessagesAsync(details.Value.token, CancellationToken.None);
        if (messages.Count == 0)
        {
            var markup = new InlineKeyboardMarkup(new [] {
                new [] { InlineKeyboardButton.WithCallbackData("🔄 Проверить еще раз", $"check_mail_by_address:{address}") },
                new [] { InlineKeyboardButton.WithCallbackData("🗑️ Удалить", $"delete_email_by_address:{address}"), InlineKeyboardButton.WithCallbackData("📋 Скопировать", $"copy_email:{address}") }
            });
            await botClient.EditMessageText(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, $"Для <code>{address}</code> нет новых писем.", parseMode: ParseMode.Html, replyMarkup: markup);
            return;
        }

        var lines = new List<string> { $"✉️ Письма для <code>{address}</code>:\n" };
        foreach (var m in messages.Take(5))
        {
            var from = m.TryGetValue("from", out var fObj) ? fObj?.ToString() : "unknown";
            var subject = m.TryGetValue("subject", out var sObj) ? sObj?.ToString() : "Без темы";
            lines.Add($"— От: <code>{from}</code> | Тема: <code>{subject}</code>");
        }
        await botClient.EditMessageText(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId, string.Join("\n", lines), parseMode: ParseMode.Html);
    }
}


