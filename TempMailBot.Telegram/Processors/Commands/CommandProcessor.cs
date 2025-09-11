// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TempMailBot.Logging.Core;

namespace TempMailBot.Telegram.Processors.Commands;

/// <summary>
/// Registers and executes command implementations based on incoming messages.
/// </summary>
public class CommandProcessor
{
    private readonly ITelegramBotClient _botClient;
    private readonly Dictionary<string, ICommand> _commands = new();
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new instance of <see cref="CommandProcessor"/>.
    /// </summary>
    public CommandProcessor(ITelegramBotClient botClient, ILogger logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    /// <summary>
    /// Registers a command and its aliases.
    /// </summary>
    public void RegisterCommand(ICommand command)
    {
        _commands[$"/{command.Name}"] = command;
        _logger.LogInformation("Registered command: /{0}", command.Name);

        if (command.Aliases == null) 
            return;
        
        foreach (var alias in command.Aliases)
        {
            _commands[$"/{alias}"] = command;
            _logger.LogInformation("Registered command alias: /{0} -> /{1}", alias, command.Name);
        }
    }
    
    /// <summary>
    /// Returns whether the command is allowed for a chat type.
    /// </summary>
    private bool IsScopeAllowed(ICommand command, ChatType chatType)
    {
        var scopeMapping = new Dictionary<CommandScope, ChatType>
        {
            { CommandScope.Private, ChatType.Private },
            { CommandScope.Group, ChatType.Group },
            { CommandScope.SuperGroup, ChatType.Supergroup }
        };

        return command.Scopes.Any(scope => scopeMapping[scope] == chatType);
    }

    /// <summary>
    /// Matches incoming message to a command and executes it.
    /// </summary>
    public async Task HandleCommandAsync(Message message)
    {
        if (_commands.TryGetValue(message.Text!, out var command))
        {
            var chat = message.Chat;

            if (IsScopeAllowed(command, chat.Type))
            {
                _logger.LogInformation("Executing command {0} in chat {1} ({2})", message.Text, chat.Id, chat.Type);
                await command.ExecuteAsync(_botClient, message);
            }
            else
            {
                _logger.LogWarning("Command {0} is not allowed in chat type {1}", message.Text, chat.Type);
            }
        }
        else
        {
            _logger.LogDebug("Unknown command received: {0}", message.Text);
        }
    }
}