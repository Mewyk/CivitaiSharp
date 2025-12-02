namespace CivitaiSharp.Core.Models;

/// <summary>
/// Time periods used to restrict results in queries (e.g. week, month).
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.ApiStringRegistry"/>.
/// </remarks>
public enum TimePeriod
{
    /// <summary>
    /// Include results from all time with no date restriction.
    /// Maps to API value "AllTime".
    /// </summary>
    AllTime,

    /// <summary>
    /// Include results from the past year only.
    /// Maps to API value "Year".
    /// </summary>
    Year,

    /// <summary>
    /// Include results from the past month only.
    /// Maps to API value "Month".
    /// </summary>
    Month,

    /// <summary>
    /// Include results from the past week only.
    /// Maps to API value "Week".
    /// </summary>
    Week,

    /// <summary>
    /// Include results from the past day only.
    /// Maps to API value "Day".
    /// </summary>
    Day
}
