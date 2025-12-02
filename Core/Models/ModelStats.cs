namespace CivitaiSharp.Core.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Statistics for a model based on user interactions and engagement.
/// </summary>
/// <param name="DownloadCount">The total number of times this model has been downloaded. Maps to JSON property "downloadCount".</param>
/// <param name="ThumbsUpCount">The total number of thumbs-up reactions on this model. Maps to JSON property "thumbsUpCount".</param>
/// <param name="ThumbsDownCount">The total number of thumbs-down reactions on this model. Maps to JSON property "thumbsDownCount".</param>
/// <param name="CommentCount">The total number of comments on this model. Maps to JSON property "commentCount".</param>
/// <param name="TippedAmountCount">The cumulative amount of tips or donations received for this model. Maps to JSON property "tippedAmountCount".</param>
public sealed record ModelStats(
    [property: JsonPropertyName("downloadCount")] int DownloadCount,
    [property: JsonPropertyName("thumbsUpCount")] int ThumbsUpCount,
    [property: JsonPropertyName("thumbsDownCount")] int ThumbsDownCount,
    [property: JsonPropertyName("commentCount")] int CommentCount,
    [property: JsonPropertyName("tippedAmountCount")] int TippedAmountCount);
