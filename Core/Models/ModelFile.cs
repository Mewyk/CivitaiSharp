namespace CivitaiSharp.Core.Models;

using System;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Models.Common;

/// <summary>
/// File associated with a model version.
/// </summary>
/// <param name="Id">The unique identifier for the file. Maps to JSON property "id".</param>
/// <param name="SizeKilobytes">The size of the model file in kilobytes. Maps to JSON property "sizeKB".</param>
/// <param name="Name">The filename of the model file. Maps to JSON property "name".</param>
/// <param name="Type">The type of file (e.g., "Model", "Training Data"). Maps to JSON property "type".</param>
/// <param name="PickleScanResult">Results of the pickle scan for security ("Pending", "Success", "Danger", "Error"). Maps to JSON property "pickleScanResult".</param>
/// <param name="PickleScanMessage">Details or error message from the pickle scan. Maps to JSON property "pickleScanMessage".</param>
/// <param name="VirusScanResult">Results of the virus scan for security ("Pending", "Success", "Danger", "Error"). Maps to JSON property "virusScanResult".</param>
/// <param name="VirusScanMessage">Details or error message from the virus scan. Maps to JSON property "virusScanMessage".</param>
/// <param name="ScannedAt">The date and time when the file was scanned for security. Maps to JSON property "scannedAt".</param>
/// <param name="Metadata">Metadata about the file format and configuration. Maps to JSON property "metadata".</param>
/// <param name="Hashes">Hash values of the file using various algorithms. Maps to JSON property "hashes".</param>
/// <param name="DownloadUrl">Direct download URL for this file. Maps to JSON property "downloadUrl".</param>
/// <param name="Primary">Indicates whether this is the primary file for the model version. Maps to JSON property "primary".</param>
public sealed record ModelFile(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("sizeKB")] double SizeKilobytes,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("pickleScanResult")] string? PickleScanResult,
    [property: JsonPropertyName("pickleScanMessage")] string? PickleScanMessage,
    [property: JsonPropertyName("virusScanResult")] string? VirusScanResult,
    [property: JsonPropertyName("virusScanMessage")] string? VirusScanMessage,
    [property: JsonPropertyName("scannedAt")] DateTime? ScannedAt,
    [property: JsonPropertyName("metadata")] FileMetadata? Metadata,
    [property: JsonPropertyName("hashes")] Hashes? Hashes,
    [property: JsonPropertyName("downloadUrl")] string? DownloadUrl,
    [property: JsonPropertyName("primary")] bool? Primary);
