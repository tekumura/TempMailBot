// Copyright (c) 2025 cybermura
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using TempMailBot.Logging.Models;

namespace TempMailBot.Logging.Filters;

/// <summary>
/// A filter that filters log entries based on logger category names using regular expressions.
/// </summary>
/// <remarks>
/// This filter uses regular expressions to match logger names and can be configured
/// to be either inclusive or exclusive.
/// </remarks>
public class CategoryFilter : ILogFilter
{
    private readonly string _pattern;
    private readonly Regex _regex;
    private readonly bool _isInclusive;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryFilter"/> class.
    /// </summary>
    /// <param name="pattern">The regular expression pattern to match against logger names.</param>
    /// <param name="isInclusive">True if matching loggers should be included; false if they should be excluded.</param>
    /// <exception cref="ArgumentNullException">Thrown when pattern is null.</exception>
    /// <exception cref="ArgumentException">Thrown when pattern is not a valid regular expression.</exception>
    public CategoryFilter(string pattern, bool isInclusive = true)
    {
        _pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        _isInclusive = isInclusive;
        
        try
        {
            _regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Invalid regex pattern: {pattern}", ex);
        }
    }

    /// <summary>
    /// Gets the name of the filter.
    /// </summary>
    /// <value>The name of the filter including the pattern and mode.</value>
    public string Name => $"CategoryFilter({_pattern}, {(IsInclusive ? "inclusive" : "exclusive")})";

    /// <summary>
    /// Gets a value indicating whether this filter is inclusive.
    /// </summary>
    /// <value>True if the filter is inclusive; otherwise, false.</value>
    public bool IsInclusive => _isInclusive;

    /// <summary>
    /// Determines whether the specified log entry should be processed based on its logger name.
    /// </summary>
    /// <param name="logEntry">The log entry to evaluate.</param>
    /// <returns>True if the log entry should be processed based on the filter configuration; otherwise, false.</returns>
    public bool IsEnabled(LogEntry logEntry)
    {
        if (logEntry == null)
            return false;

        var isMatch = _regex.IsMatch(logEntry.LoggerName);
        return _isInclusive ? isMatch : !isMatch;
    }
}