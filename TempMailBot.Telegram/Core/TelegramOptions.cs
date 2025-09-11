// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace TempMailBot.Telegram.Core;

/// <summary>
/// Options for configuring the Telegram bot instance.
/// </summary>
public class TelegramOptions
{
    /// <summary>
    /// Bot token issued by @BotFather.
    /// </summary>
    public string Token { get; set; } = string.Empty;
    /// <summary>
    /// If true, drops pending updates on start.
    /// </summary>
    public bool DropPendingUpdates { get; set; } = true;
}