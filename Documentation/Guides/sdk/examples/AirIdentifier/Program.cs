using System;
using CivitaiSharp.Sdk.Air;

#region ParseAir
var air = AirIdentifier.Parse("urn:air:sdxl:lora:civitai:328553@368189");

Console.WriteLine($"Ecosystem: {air.Ecosystem}");
Console.WriteLine($"Asset Type: {air.AssetType}");
Console.WriteLine($"Source: {air.Source}");
Console.WriteLine($"Model ID: {air.ModelId}");
Console.WriteLine($"Version ID: {air.VersionId}");
#endregion

#region CreateAir
var airIdentifier = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Lora,
    AirSource.Civitai,
    328553,
    368189);

Console.WriteLine(airIdentifier.ToString());
#endregion

#region BuilderPattern
var builtAir = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Lora)
    .WithSource(AirSource.HuggingFace)
    .WithModelId(328553)
    .WithVersionId(368189)
    .Build();

Console.WriteLine(builtAir.ToString());
#endregion

#region ValidateAir
if (AirIdentifier.TryParse("urn:air:sdxl:lora:civitai:328553@368189", out var validAir))
{
    Console.WriteLine($"Valid AIR: {validAir}");
}
else
{
    Console.WriteLine("Invalid AIR format");
}
#endregion

#region multi-provider
var civitaiCheckpoint = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Checkpoint,
    AirSource.Civitai,
    101055,
    128078);

var huggingFaceCheckpoint = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Checkpoint,
    AirSource.HuggingFace,
    100,
    200);

var airBuilder = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithModelId(101055)
    .WithVersionId(128078);

var civitaiAir = airBuilder.WithSource(AirSource.Civitai).Build();
var leonardo = airBuilder.WithSource(AirSource.Leonardo).Build();
#endregion
