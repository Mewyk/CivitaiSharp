namespace Guides.AirIdentifier;

using System;
using CivitaiSharp.Sdk.Air;

public static class AirIdentifierSamples
{
    public static void Main()
    {
    }

    public static void ParseExample()
    {
        #region parse-air
        var air = AirIdentifier.Parse("urn:air:sdxl:lora:civitai:328553@368189");

        Console.WriteLine($"Ecosystem: {air.Ecosystem}");  // StableDiffusionXl
        Console.WriteLine($"Asset Type: {air.AssetType}"); // Lora
        Console.WriteLine($"Source: {air.Source}");        // Civitai
        Console.WriteLine($"Model ID: {air.ModelId}");     // 328553
        Console.WriteLine($"Version ID: {air.VersionId}"); // 368189
        #endregion parse-air
    }

    public static void CreateExample()
    {
        #region create-air
        // Using the constructor
        var air = new AirIdentifier(
            AirEcosystem.StableDiffusionXl,
            AirAssetType.Lora,
            AirSource.Civitai,
            328553,
            368189);

        Console.WriteLine(air.ToString());
        // Output: urn:air:sdxl:lora:civitai:328553@368189

        // Using the factory method (defaults to Civitai source)
        var air2 = AirIdentifier.Create(
            AirEcosystem.StableDiffusionXl,
            AirAssetType.Lora,
            328553,
            368189);
        #endregion create-air
    }

    public static void BuilderExample()
    {
        #region builder-pattern
        var air = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithSource(AirSource.HuggingFace)
            .WithModelId(328553)
            .WithVersionId(368189)
            .Build();

        Console.WriteLine(air.ToString());
        // Output: urn:air:sdxl:lora:huggingface:328553@368189
        #endregion builder-pattern
    }

    public static void ValidationExample()
    {
        #region validate-air
        if (AirIdentifier.TryParse("urn:air:sdxl:lora:civitai:328553@368189", out var air))
        {
            Console.WriteLine($"Valid AIR: {air}");
        }
        else
        {
            Console.WriteLine("Invalid AIR format");
        }
        #endregion validate-air
    }

    public static void MultiProviderExample()
    {
        #region multi-provider
        // Civitai resource
        var civitaiCheckpoint = new AirIdentifier(
            AirEcosystem.StableDiffusionXl,
            AirAssetType.Checkpoint,
            AirSource.Civitai,
            101055,
            128078);

        // Hugging Face resource
        var huggingFaceCheckpoint = new AirIdentifier(
            AirEcosystem.StableDiffusionXl,
            AirAssetType.Checkpoint,
            AirSource.HuggingFace,
            100,
            200);

        // Switching sources programmatically
        var airBuilder = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Checkpoint)
            .WithModelId(101055)
            .WithVersionId(128078);

        var civitaiAir = airBuilder.WithSource(AirSource.Civitai).Build();
        var leonardo = airBuilder.WithSource(AirSource.Leonardo).Build();
        #endregion multi-provider
    }
}
