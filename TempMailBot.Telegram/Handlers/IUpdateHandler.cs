// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Telegram.Bot.Types;

namespace TempMailBot.Telegram.Handlers;

/// <summary>
/// Abstraction for handling specific Telegram updates.
/// </summary>
public interface IUpdateHandler
{
    /// <summary>
    /// Attempts to handle an update.
    /// </summary>
    /// <param name="update">Telegram update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if handled, otherwise false.</returns>
    Task<bool> HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}