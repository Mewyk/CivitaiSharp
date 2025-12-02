namespace CivitaiSharp.Core.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Statistics for an image based on user reactions and interactions.
/// </summary>
/// <param name="CryCount">The number of cry reaction emoji reactions on the image. Maps to JSON property "cryCount".</param>
/// <param name="LaughCount">The number of laugh reaction emoji reactions on the image. Maps to JSON property "laughCount".</param>
/// <param name="LikeCount">The number of like reactions on the image. Maps to JSON property "likeCount".</param>
/// <param name="DislikeCount">The number of dislike reactions on the image. Maps to JSON property "dislikeCount".</param>
/// <param name="HeartCount">The number of heart reactions on the image. Maps to JSON property "heartCount".</param>
/// <param name="CommentCount">The number of comments on the image. Maps to JSON property "commentCount".</param>
public sealed record ImageStats(
    [property: JsonPropertyName("cryCount")] int? CryCount,
    [property: JsonPropertyName("laughCount")] int? LaughCount,
    [property: JsonPropertyName("likeCount")] int? LikeCount,
    [property: JsonPropertyName("dislikeCount")] int? DislikeCount,
    [property: JsonPropertyName("heartCount")] int? HeartCount,
    [property: JsonPropertyName("commentCount")] int? CommentCount);
