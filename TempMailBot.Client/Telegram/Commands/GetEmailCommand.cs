// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TempMailBot.Telegram.Processors.Commands;
using TempMailBot.Logging.Core;
using TempMailBot.Client.Data;
using TempMailBot.Client.Services;

namespace TempMailBot.Client.Telegram.Commands;

public class GetEmailCommand : ICommand
{
    public string Name => "get_email";
    public IEnumerable<CommandScope> Scopes => new[] { CommandScope.Private };

    private readonly SqliteRepository _repo;
    private readonly MailTmService _mail;
    private readonly ILogger _logger;
    private readonly Random _random = new Random();

    public GetEmailCommand(SqliteRepository repo, MailTmService mail, ILogger logger)
    {
        _repo = repo;
        _mail = mail;
        _logger = logger;
    }

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        var userId = message.From!.Id;
        _repo.EnsureUser(userId);
        var (count, lastReset) = _repo.GetUserQuota(userId);
        if (count >= SqliteRepository.WeeklyEmailLimitFree)
        {
            var resetAt = lastReset.AddDays(7);
            await botClient.SendMessage(message.Chat.Id, $"🚫 Лимит {SqliteRepository.WeeklyEmailLimitFree} email в неделю. Сброс: {resetAt:dd.MM.yyyy HH:mm}");
            return;
        }

        await botClient.SendMessage(message.Chat.Id, "🔮 Генерирую временный email... подождите");

        var domains = await _mail.GetDomainsAsync(CancellationToken.None);
        if (domains.Count == 0)
        {
            await botClient.SendMessage(message.Chat.Id, "Mail.tm недоступен. Попробуйте позже.");
            return;
        }
        var domain = domains[_random.Next(domains.Count)];
        var prefix = GenerateString(12);
        var address = $"{prefix}@{domain}";
        var password = GenerateString(16);

        var created = await _mail.CreateAccountAsync(address, password, CancellationToken.None);
        if (!created)
        {
            await botClient.SendMessage(message.Chat.Id, "Не удалось создать аккаунт. Попробуйте еще раз.");
            return;
        }

        var token = await _mail.AuthenticateAsync(address, password, CancellationToken.None);
        if (string.IsNullOrEmpty(token))
        {
            await botClient.SendMessage(message.Chat.Id, "Не удалось аутентифицироваться. Попробуйте еще раз.");
            return;
        }

        if (_repo.AddTempEmail(userId, address, password, token))
        {
            _repo.UpdateUserEmailCount(userId, count + 1, lastReset);
            
            var buttons = new InlineKeyboardMarkup(new[] {
                new[] { InlineKeyboardButton.WithCallbackData("✉️ Проверить почту", $"check_mail_by_address:{address}") },
                new[] { InlineKeyboardButton.WithCallbackData("🗑️ Удалить", $"delete_email_by_address:{address}"), InlineKeyboardButton.WithCallbackData("📋 Скопировать", $"copy_email:{address}") }
            });

            await botClient.SendMessage(
                message.Chat.Id,
                $"🎉 Ваш email: <code>{address}</code>\nПароль (для API): <spoiler>{password}</spoiler>",
                parseMode: ParseMode.Html,
                replyMarkup: buttons
            );
            _logger.LogInformation("Создан email для {0}: {1}", userId, address);
        }
        else
        {
            await botClient.SendMessage(message.Chat.Id, "Ошибка сохранения email. Попробуйте позже.");
        }
    }

    private static string GenerateString(int len)
    {
        const string letters = "abcdefghijklmnopqrstuvwxyz0123456789";
        var chars = new char[len];
        var rng = Random.Shared;
        for (int i = 0; i < len; i++) chars[i] = letters[rng.Next(letters.Length)];
        return new string(chars);
    }
}


