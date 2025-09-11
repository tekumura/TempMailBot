// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Data.SqlClient;
using TempMailBot.Logging.Formatters;
using TempMailBot.Logging.Models;

namespace TempMailBot.Logging.Handlers;

/// <summary>
/// A log handler that outputs log entries to a database.
/// </summary>
/// <remarks>
/// This handler writes log entries to a SQL Server database table. It includes
/// support for creating the log table if it doesn't exist and handles database
/// connection management.
/// </remarks>
public class DatabaseLogHandler : ILogHandler
{
    private readonly string _connectionString;
    private readonly string _tableName;
    private ILogFormatter _formatter;
    private readonly object _lock = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseLogHandler"/> class.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="tableName">The name of the table to store log entries.</param>
    /// <param name="formatter">The formatter to use for formatting log entries.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public DatabaseLogHandler(string connectionString, string tableName, ILogFormatter formatter)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    /// <summary>
    /// Gets or sets a value indicating whether this handler is enabled.
    /// </summary>
    /// <value>True if the handler is enabled; otherwise, false.</value>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Handles a log entry by writing it to the database.
    /// </summary>
    /// <param name="logEntry">The log entry to handle.</param>
    public void Handle(LogEntry logEntry)
    {
        if (!IsEnabled)
            return;

        lock (_lock)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $@"
                    INSERT INTO {_tableName} (LoggerName, Level, Message, Exception, Timestamp, Properties)
                    VALUES (@LoggerName, @Level, @Message, @Exception, @Timestamp, @Properties)";

                command.Parameters.AddWithValue("@LoggerName", logEntry.LoggerName);
                command.Parameters.AddWithValue("@Level", logEntry.Level.ToString());
                command.Parameters.AddWithValue("@Message", logEntry.Message);
                command.Parameters.AddWithValue("@Exception", logEntry.Exception?.ToString() ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Timestamp", logEntry.Timestamp);
                command.Parameters.AddWithValue("@Properties", System.Text.Json.JsonSerializer.Serialize(logEntry.Properties));

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Log the error to console to avoid infinite loops
                Console.WriteLine($"Error writing to database: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Sets the formatter to use for formatting log entries.
    /// </summary>
    /// <param name="formatter">The formatter to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when formatter is null.</exception>
    public void SetFormatter(ILogFormatter formatter)
    {
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    /// <summary>
    /// Creates the log table in the database if it doesn't exist.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="tableName">The name of the table to create.</param>
    public static void CreateTable(string connectionString, string tableName)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = $@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{tableName}' AND xtype='U')
            CREATE TABLE {tableName} (
                Id BIGINT IDENTITY(1,1) PRIMARY KEY,
                LoggerName NVARCHAR(255) NOT NULL,
                Level NVARCHAR(50) NOT NULL,
                Message NVARCHAR(MAX) NOT NULL,
                Exception NVARCHAR(MAX),
                Timestamp DATETIME2 NOT NULL,
                Properties NVARCHAR(MAX)
            )";

        command.ExecuteNonQuery();
    }
}
