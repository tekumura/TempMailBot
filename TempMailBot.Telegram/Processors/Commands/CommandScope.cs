// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace TempMailBot.Telegram.Processors.Commands;

/// <summary>
/// Chat scopes where commands are valid.
/// </summary>
public enum CommandScope
{
    Private,
    Group,
    SuperGroup
}