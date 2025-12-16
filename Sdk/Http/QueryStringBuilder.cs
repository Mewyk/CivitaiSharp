namespace CivitaiSharp.Sdk.Http;

using System;
using System.Globalization;
using System.Text;

/// <summary>
/// Builder for constructing URL query strings with proper encoding.
/// This class is mutable and not thread-safe.
/// </summary>
/// <example>
/// <code>
/// var query = new QueryStringBuilder()
///     .Append("token", "abc123")
///     .AppendIf("wait", true)
///     .Append("startDate", DateTime.Now)
///     .BuildUri("/api/v1/jobs");
/// // Result: "/api/v1/jobs?token=abc123&amp;wait=true&amp;startDate=2025-12-06T19:00:00.0000000Z"
/// </code>
/// </example>
internal sealed class QueryStringBuilder
{
    private readonly StringBuilder _builder = new();

    /// <summary>
    /// Appends a parameter to the query string if the value is not null or whitespace.
    /// </summary>
    /// <param name="key">The parameter key.</param>
    /// <param name="value">The parameter value.</param>
    /// <returns>This builder for method chaining.</returns>
    public QueryStringBuilder Append(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return this;
        }

        AppendSeparatorIfNeeded();
        _builder.Append(Uri.EscapeDataString(key));
        _builder.Append('=');
        _builder.Append(Uri.EscapeDataString(value));
        return this;
    }

    /// <summary>
    /// Appends a boolean parameter to the query string if the condition is true.
    /// </summary>
    /// <param name="key">The parameter key.</param>
    /// <param name="condition">If true, appends the parameter with value "true".</param>
    /// <returns>This builder for method chaining.</returns>
    public QueryStringBuilder AppendIf(string key, bool condition)
    {
        if (condition)
        {
            Append(key, "true");
        }
        return this;
    }

    /// <summary>
    /// Appends a boolean parameter with explicit true/false value.
    /// </summary>
    /// <param name="key">The parameter key.</param>
    /// <param name="value">The boolean value to append.</param>
    /// <returns>This builder for method chaining.</returns>
    public QueryStringBuilder Append(string key, bool? value)
    {
        if (value.HasValue)
        {
            Append(key, value.Value ? "true" : "false");
        }
        return this;
    }

    /// <summary>
    /// Appends a date parameter in ISO 8601 format.
    /// </summary>
    /// <param name="key">The parameter key.</param>
    /// <param name="value">The date value.</param>
    /// <returns>This builder for method chaining.</returns>
    public QueryStringBuilder Append(string key, DateTime? value)
    {
        if (value.HasValue)
        {
            Append(key, value.Value.ToString("O", CultureInfo.InvariantCulture));
        }
        return this;
    }

    /// <summary>
    /// Appends an integer parameter.
    /// </summary>
    /// <param name="key">The parameter key.</param>
    /// <param name="value">The integer value.</param>
    /// <returns>This builder for method chaining.</returns>
    public QueryStringBuilder Append(string key, int? value)
    {
        if (value.HasValue)
        {
            Append(key, value.Value.ToString(CultureInfo.InvariantCulture));
        }
        return this;
    }

    /// <summary>
    /// Appends a long integer parameter.
    /// </summary>
    /// <param name="key">The parameter key.</param>
    /// <param name="value">The long integer value.</param>
    /// <returns>This builder for method chaining.</returns>
    public QueryStringBuilder Append(string key, long? value)
    {
        if (value.HasValue)
        {
            Append(key, value.Value.ToString(CultureInfo.InvariantCulture));
        }
        return this;
    }

    /// <summary>
    /// Clears all parameters from the builder, allowing reuse.
    /// </summary>
    /// <returns>This builder for method chaining.</returns>
    public QueryStringBuilder Clear()
    {
        _builder.Clear();
        return this;
    }

    /// <summary>
    /// Builds the final URI by combining the base path with the query string.
    /// </summary>
    /// <param name="basePath">The base URI path.</param>
    /// <returns>The complete URI with query string if parameters exist.</returns>
    public string BuildUri(string basePath)
    {
        return _builder.Length > 0 ? $"{basePath}?{_builder}" : basePath;
    }

    /// <summary>
    /// Gets the current length of the query string.
    /// </summary>
    public int Length => _builder.Length;

    /// <inheritdoc />
    public override string ToString() => _builder.ToString();

    private void AppendSeparatorIfNeeded()
    {
        if (_builder.Length > 0)
        {
            _builder.Append('&');
        }
    }
}
