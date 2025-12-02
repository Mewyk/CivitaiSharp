namespace CivitaiSharp.Sdk.Models.Usage;

using System;
using System.Text.Json.Serialization;

/// <summary>
/// Account usage and consumption statistics.
/// </summary>
/// <param name="TotalCost">The total Buzz spent in the period. Maps to JSON property "totalCost".</param>
/// <param name="TotalCredits">The total credits allocated to the account. Maps to JSON property "totalCredits".</param>
/// <param name="RemainingCredits">The remaining credits available. Maps to JSON property "remainingCredits".</param>
/// <param name="PeriodStart">The start of the reporting period. Maps to JSON property "periodStart".</param>
/// <param name="PeriodEnd">The end of the reporting period. Maps to JSON property "periodEnd".</param>
/// <param name="JobCount">The number of jobs submitted in the period. Maps to JSON property "jobCount".</param>
/// <param name="AverageCostPerJob">The average cost per job in the period. Maps to JSON property "averageCostPerJob".</param>
public sealed record ConsumptionDetails(
    [property: JsonPropertyName("totalCost")] decimal TotalCost,
    [property: JsonPropertyName("totalCredits")] decimal TotalCredits,
    [property: JsonPropertyName("remainingCredits")] decimal RemainingCredits,
    [property: JsonPropertyName("periodStart")] DateTime? PeriodStart,
    [property: JsonPropertyName("periodEnd")] DateTime? PeriodEnd,
    [property: JsonPropertyName("jobCount")] int JobCount,
    [property: JsonPropertyName("averageCostPerJob")] decimal AverageCostPerJob);
