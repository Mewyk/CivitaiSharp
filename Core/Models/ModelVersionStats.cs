namespace CivitaiSharp.Core.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Statistics for a model version based on user interactions and engagement.
/// </summary>
/// <param name="DownloadCount">The total number of times this model version has been downloaded. Maps to JSON property "downloadCount".</param>
/// <param name="ThumbsUpCount">The total number of thumbs-up reactions on this model version. Maps to JSON property "thumbsUpCount".</param>
/// <param name="ThumbsDownCount">The total number of thumbs-down reactions on this model version. Maps to JSON property "thumbsDownCount".</param>
public sealed record ModelVersionStats(
    [property: JsonPropertyName("downloadCount")] int DownloadCount,
    [property: JsonPropertyName("thumbsUpCount")] int ThumbsUpCount,
    [property: JsonPropertyName("thumbsDownCount")] int? ThumbsDownCount);
