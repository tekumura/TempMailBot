// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TempMailBot.Telegram.Processors.Commands;

namespace TempMailBot.Client.Telegram.Commands;

/// <summary>
/// Provides a basic help message explaining available commands and features.
/// </summary>
public class HelpCommand : ICommand
{
    public string Name => "help";
    public IEnumerable<CommandScope> Scopes => new[] { CommandScope.Private };
    
    /// <summary>
    /// Sends the help text to the current chat.
    /// </summary>
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        var text =
            "❓ <b>Как пользоваться ботом:</b>\n\n" +
            "✨ <b>Получить Email:</b> Создает новый временный Email-адрес.\n" +
            "📨 <b>Мои Email'ы:</b> Показывает список всех ваших активных временных Email-адресов. Здесь вы можете проверить их на наличие новых писем или удалить.";

        await botClient.SendMessage(
            message.Chat.Id,
            text,
            parseMode: ParseMode.Html);
    }
}