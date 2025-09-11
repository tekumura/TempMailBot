// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.Data.Sqlite;
using TempMailBot.Logging.Core;

namespace TempMailBot.Client.Data;

/// <summary>
/// Repository for accessing and modifying bot data stored in SQLite.
/// </summary>
public class SqliteRepository
{
    private readonly string _connectionString;
    private readonly ILogger _logger;

    public const int WeeklyEmailLimitFree = 10;

    /// <summary>
    /// Initializes the repository and database schema.
    /// </summary>
    /// <param name="logger">Shared logger instance.</param>
    public SqliteRepository(ILogger logger)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "bot_data.db");
        _connectionString = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();
        _logger = logger;
        Initialize();
    }

    /// <summary>
    /// Creates database tables if they do not exist.
    /// </summary>
    private void Initialize()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS users (
    user_id INTEGER PRIMARY KEY,
    emails_created_this_week INTEGER DEFAULT 0,
    last_week_reset_at TEXT
);
CREATE TABLE IF NOT EXISTS temp_emails (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER,
    address TEXT UNIQUE,
    password TEXT,
    mailtm_token TEXT,
    created_at TEXT,
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);";
        cmd.ExecuteNonQuery();
        _logger.LogInformation("SQLite initialized");
    }

    /// <summary>
    /// Ensures a user row exists for a given user identifier.
    /// </summary>
    /// <param name="userId">Telegram user identifier.</param>
    public void EnsureUser(long userId)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var tx = conn.BeginTransaction();
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = "INSERT OR IGNORE INTO users (user_id, last_week_reset_at) VALUES ($uid, $now)";
        cmd.Parameters.AddWithValue("$uid", userId);
        cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("o"));
        cmd.ExecuteNonQuery();
        tx.Commit();
    }

    /// <summary>
    /// Returns the user's weekly email creation count and the last reset timestamp.
    /// If a week elapsed, resets the counter and returns zeros.
    /// </summary>
    /// <param name="userId">Telegram user identifier.</param>
    /// <returns>Tuple of (emailsCreatedThisWeek, lastResetAtUtc).</returns>
    public (int emailsCreatedThisWeek, DateTime lastResetAtUtc) GetUserQuota(long userId)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT emails_created_this_week, last_week_reset_at FROM users WHERE user_id=$uid";
        cmd.Parameters.AddWithValue("$uid", userId);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return (0, DateTime.UtcNow);
        var count = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
        var last = reader.IsDBNull(1) ? DateTime.UtcNow : DateTime.Parse(reader.GetString(1));
        // Weekly reset
        if (DateTime.UtcNow - last > TimeSpan.FromDays(7))
        {
            UpdateUserEmailCount(userId, 0, DateTime.UtcNow);
            return (0, DateTime.UtcNow);
        }
        return (count, last);
    }

    /// <summary>
    /// Updates the user's weekly email creation counter.
    /// </summary>
    /// <param name="userId">Telegram user identifier.</param>
    /// <param name="count">New count value.</param>
    /// <param name="resetTimeUtc">Reset timestamp (UTC).</param>
    public void UpdateUserEmailCount(long userId, int count, DateTime resetTimeUtc)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE users SET emails_created_this_week=$c, last_week_reset_at=$t WHERE user_id=$u";
        cmd.Parameters.AddWithValue("$c", count);
        cmd.Parameters.AddWithValue("$t", resetTimeUtc.ToString("o"));
        cmd.Parameters.AddWithValue("$u", userId);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Stores a new temporary email for a user.
    /// </summary>
    /// <param name="userId">Telegram user identifier.</param>
    /// <param name="address">Email address.</param>
    /// <param name="password">Password used for Mail.tm API.</param>
    /// <param name="token">Mail.tm JWT token.</param>
    /// <returns>True if inserted, false if duplicate.</returns>
    public bool AddTempEmail(long userId, string address, string password, string token)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var tx = conn.BeginTransaction();
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = "INSERT INTO temp_emails (user_id, address, password, mailtm_token, created_at) VALUES ($u, $a, $p, $t, $c)";
        cmd.Parameters.AddWithValue("$u", userId);
        cmd.Parameters.AddWithValue("$a", address);
        cmd.Parameters.AddWithValue("$p", password);
        cmd.Parameters.AddWithValue("$t", token);
        cmd.Parameters.AddWithValue("$c", DateTime.UtcNow.ToString("o"));
        try
        {
            cmd.ExecuteNonQuery();
            tx.Commit();
            return true;
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            _logger.LogWarning("Duplicate email address attempt: {0}", address);
            return false;
        }
    }

    /// <summary>
    /// Retrieves a list of user's temporary emails.
    /// </summary>
    /// <param name="userId">Telegram user identifier.</param>
    /// <returns>List of (id, address, createdAtUtc).</returns>
    public List<(int id, string address, DateTime createdAtUtc)> GetUserEmails(long userId)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, address, created_at FROM temp_emails WHERE user_id=$u ORDER BY id DESC";
        cmd.Parameters.AddWithValue("$u", userId);
        using var reader = cmd.ExecuteReader();
        var result = new List<(int, string, DateTime)>();
        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var addr = reader.GetString(1);
            var created = DateTime.Parse(reader.GetString(2));
            result.Add((id, addr, created));
        }
        return result;
    }

    /// <summary>
    /// Returns full email details by address for a given user.
    /// </summary>
    /// <param name="userId">Telegram user identifier.</param>
    /// <param name="address">Email address.</param>
    /// <returns>Details tuple or null.</returns>
    public (int id, long userId, string address, string password, string token, DateTime createdAtUtc)? GetEmailByAddress(long userId, string address)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, user_id, address, password, mailtm_token, created_at FROM temp_emails WHERE user_id=$u AND address=$a";
        cmd.Parameters.AddWithValue("$u", userId);
        cmd.Parameters.AddWithValue("$a", address);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return (
            reader.GetInt32(0),
            reader.GetInt64(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetString(4),
            DateTime.Parse(reader.GetString(5))
        );
    }

    /// <summary>
    /// Deletes an email by address for a given user.
    /// </summary>
    /// <param name="userId">Telegram user identifier.</param>
    /// <param name="address">Email address to delete.</param>
    public void DeleteEmailByAddress(long userId, string address)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM temp_emails WHERE user_id=$u AND address=$a";
        cmd.Parameters.AddWithValue("$u", userId);
        cmd.Parameters.AddWithValue("$a", address);
        cmd.ExecuteNonQuery();
    }
}


