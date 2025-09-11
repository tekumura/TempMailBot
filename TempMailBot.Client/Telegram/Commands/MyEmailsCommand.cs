// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TempMailBot.Telegram.Processors.Commands;
using TempMailBot.Client.Data;

namespace TempMailBot.Client.Telegram.Commands;

/// <summary>
/// Command that lists user-created temporary emails and provides action buttons.
/// </summary>
public class MyEmailsCommand : ICommand
{
    public string Name => "my_emails";
    public IEnumerable<CommandScope> Scopes => new[] { CommandScope.Private };

    private readonly SqliteRepository _repo;

    /// <summary>
    /// Initializes a new instance of <see cref="MyEmailsCommand"/>.
    /// </summary>
    public MyEmailsCommand(SqliteRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Sends a formatted list of emails with inline buttons.
    /// </summary>
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        var userId = message.From!.Id;
        var emails = _repo.GetUserEmails(userId);
        if (emails.Count == 0)
        {
            await botClient.SendMessage(message.Chat.Id, "У вас пока нет email. Используйте /get_email");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("📧 Ваши временные email:");
        var rows = new List<InlineKeyboardButton[]>();
        foreach (var e in emails)
        {
            sb.AppendLine($"<code>{e.address}</code> (создан: {e.createdAtUtc:dd.MM.yyyy HH:mm})");
            rows.Add(new [] { InlineKeyboardButton.WithCallbackData($"✉️ {e.address}", $"check_mail_by_address:{e.address}"), InlineKeyboardButton.WithCallbackData("🗑️", $"delete_email_by_address:{e.address}") });
        }
        await botClient.SendMessage(message.Chat.Id, sb.ToString(), parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(rows));
    }
}


