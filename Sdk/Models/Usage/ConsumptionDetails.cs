namespace CivitaiSharp.Sdk.Models.Usage;

using System;
using System.Text.Json.Serialization;

/// <summary>
/// Account consumption statistics.
/// </summary>
/// <remarks>
/// This model matches the official OpenAPI schema used by the Civitai Python and JavaScript SDKs.
/// </remarks>
/// <param name="Images">Total number of images generated in the period. Maps to JSON property "images".</param>
/// <param name="TotalCost">Total Buzz spent in the period. Maps to JSON property "totalCost".</param>
/// <param name="StartDate">Start date for the reporting period. Maps to JSON property "startDate".</param>
/// <param name="EndDate">End date for the reporting period. Maps to JSON property "endDate".</param>
public sealed record ConsumptionDetails(
    [property: JsonPropertyName("images")] int? Images,
    [property: JsonPropertyName("totalCost")] decimal? TotalCost,
    [property: JsonPropertyName("startDate")] DateTime? StartDate,
    [property: JsonPropertyName("endDate")] DateTime? EndDate);
