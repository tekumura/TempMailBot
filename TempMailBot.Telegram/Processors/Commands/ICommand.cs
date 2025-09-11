// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Telegram.Bot;
using Telegram.Bot.Types;

namespace TempMailBot.Telegram.Processors.Commands;

/// <summary>
/// Defines a Telegram bot command contract.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Primary command name without leading slash.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Optional aliases without leading slash.
    /// </summary>
    IEnumerable<string>? Aliases => null;
    /// <summary>
    /// Allowed chat scopes for the command.
    /// </summary>
    IEnumerable<CommandScope> Scopes { get; }
    /// <summary>
    /// Executes the command logic.
    /// </summary>
    Task ExecuteAsync(ITelegramBotClient botClient, Message message);
}