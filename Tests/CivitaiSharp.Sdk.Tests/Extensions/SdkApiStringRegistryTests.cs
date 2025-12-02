namespace CivitaiSharp.Sdk.Tests.Extensions;

using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Enums;
using Xunit;

/// <summary>
/// Tests for SDK enum to API string conversions.
/// Verifies that all SDK enums are properly registered with the ApiStringRegistry.
/// </summary>
public sealed class SdkApiStringRegistryTests : IClassFixture<SdkTestFixture>
{
    #region AirEcosystem Tests

    [Theory]
    [InlineData(AirEcosystem.StableDiffusion1, "sd1")]
    [InlineData(AirEcosystem.StableDiffusion2, "sd2")]
    [InlineData(AirEcosystem.StableDiffusionXl, "sdxl")]
    [InlineData(AirEcosystem.Flux1, "flux1")]
    [InlineData(AirEcosystem.Pony, "pony")]
    public void WhenConvertingAirEcosystemToApiStringThenReturnsCorrectValue(
        AirEcosystem ecosystem,
        string expected)
    {
        // Act
        var result = ecosystem.ToApiString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("sd1", AirEcosystem.StableDiffusion1)]
    [InlineData("sd2", AirEcosystem.StableDiffusion2)]
    [InlineData("sdxl", AirEcosystem.StableDiffusionXl)]
    [InlineData("flux1", AirEcosystem.Flux1)]
    [InlineData("pony", AirEcosystem.Pony)]
    [InlineData("SDXL", AirEcosystem.StableDiffusionXl)]
    [InlineData("Flux1", AirEcosystem.Flux1)]
    public void WhenParsingAirEcosystemFromApiStringThenReturnsCorrectValue(
        string apiString,
        AirEcosystem expected)
    {
        // Act
        var success = EnumExtensions.TryParseFromApiString<AirEcosystem>(apiString, out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(expected, result);
    }

    #endregion

    #region AirAssetType Tests

    [Theory]
    [InlineData(AirAssetType.Checkpoint, "checkpoint")]
    [InlineData(AirAssetType.Lora, "lora")]
    [InlineData(AirAssetType.Lycoris, "lycoris")]
    [InlineData(AirAssetType.Vae, "vae")]
    [InlineData(AirAssetType.Embedding, "embedding")]
    [InlineData(AirAssetType.Hypernetwork, "hypernet")]
    public void WhenConvertingAirAssetTypeToApiStringThenReturnsCorrectValue(
        AirAssetType assetType,
        string expected)
    {
        // Act
        var result = assetType.ToApiString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("checkpoint", AirAssetType.Checkpoint)]
    [InlineData("lora", AirAssetType.Lora)]
    [InlineData("lycoris", AirAssetType.Lycoris)]
    [InlineData("vae", AirAssetType.Vae)]
    [InlineData("embedding", AirAssetType.Embedding)]
    [InlineData("hypernet", AirAssetType.Hypernetwork)]
    [InlineData("LORA", AirAssetType.Lora)]
    public void WhenParsingAirAssetTypeFromApiStringThenReturnsCorrectValue(
        string apiString,
        AirAssetType expected)
    {
        // Act
        var success = EnumExtensions.TryParseFromApiString<AirAssetType>(apiString, out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(expected, result);
    }

    #endregion

    #region Scheduler Tests

    [Theory]
    [InlineData(Scheduler.Euler, "euler")]
    [InlineData(Scheduler.EulerAncestral, "euler_a")]
    [InlineData(Scheduler.LinearMultistep, "lms")]
    [InlineData(Scheduler.Heun, "heun")]
    [InlineData(Scheduler.DpmSolver2, "dpm_2")]
    [InlineData(Scheduler.DpmSolver2Ancestral, "dpm_2_a")]
    [InlineData(Scheduler.DpmPlusPlus2SAncestral, "dpmpp_2s_a")]
    [InlineData(Scheduler.DpmPlusPlus2M, "dpmpp_2m")]
    [InlineData(Scheduler.DpmPlusPlusSde, "dpmpp_sde")]
    [InlineData(Scheduler.DpmPlusPlus2MSde, "dpmpp_2m_sde")]
    [InlineData(Scheduler.DpmPlusPlus2MSdeKarras, "dpmpp_2m_sde_karras")]
    [InlineData(Scheduler.DpmPlusPlus3MSde, "dpmpp_3m_sde")]
    [InlineData(Scheduler.DpmPlusPlus3MSdeKarras, "dpmpp_3m_sde_karras")]
    [InlineData(Scheduler.Ddim, "ddim")]
    [InlineData(Scheduler.Plms, "plms")]
    [InlineData(Scheduler.UniPc, "uni_pc")]
    [InlineData(Scheduler.UniPcBh2, "uni_pc_bh2")]
    [InlineData(Scheduler.Ddpm, "ddpm")]
    [InlineData(Scheduler.Lcm, "lcm")]
    public void WhenConvertingSchedulerToApiStringThenReturnsCorrectValue(
        Scheduler scheduler,
        string expected)
    {
        // Act
        var result = scheduler.ToApiString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("euler", Scheduler.Euler)]
    [InlineData("euler_a", Scheduler.EulerAncestral)]
    [InlineData("dpmpp_2m_sde_karras", Scheduler.DpmPlusPlus2MSdeKarras)]
    [InlineData("EULER", Scheduler.Euler)]
    public void WhenParsingSchedulerFromApiStringThenReturnsCorrectValue(
        string apiString,
        Scheduler expected)
    {
        // Act
        var success = EnumExtensions.TryParseFromApiString<Scheduler>(apiString, out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(expected, result);
    }

    #endregion

    #region AvailabilityStatus Tests

    [Theory]
    [InlineData(AvailabilityStatus.Available, "Available")]
    [InlineData(AvailabilityStatus.Unavailable, "Unavailable")]
    [InlineData(AvailabilityStatus.Degraded, "Degraded")]
    public void WhenConvertingAvailabilityStatusToApiStringThenReturnsCorrectValue(
        AvailabilityStatus status,
        string expected)
    {
        // Act
        var result = status.ToApiString();

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region NetworkType Tests

    [Theory]
    [InlineData(NetworkType.Lora, "lora")]
    [InlineData(NetworkType.Lycoris, "lycoris")]
    [InlineData(NetworkType.Dora, "dora")]
    [InlineData(NetworkType.Embedding, "embedding")]
    [InlineData(NetworkType.Vae, "vae")]
    public void WhenConvertingNetworkTypeToApiStringThenReturnsCorrectValue(
        NetworkType networkType,
        string expected)
    {
        // Act
        var result = networkType.ToApiString();

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region ControlNetPreprocessor Tests

    [Theory]
    [InlineData(ControlNetPreprocessor.Canny, "canny")]
    [InlineData(ControlNetPreprocessor.Depth, "depth")]
    [InlineData(ControlNetPreprocessor.DepthLeres, "depth_leres")]
    [InlineData(ControlNetPreprocessor.DepthMidas, "depth_midas")]
    [InlineData(ControlNetPreprocessor.DepthZoe, "depth_zoe")]
    [InlineData(ControlNetPreprocessor.SoftEdgeHed, "softedge_hed")]
    [InlineData(ControlNetPreprocessor.SoftEdgePidinet, "softedge_pidinet")]
    [InlineData(ControlNetPreprocessor.Lineart, "lineart")]
    [InlineData(ControlNetPreprocessor.LineartAnime, "lineart_anime")]
    [InlineData(ControlNetPreprocessor.Openpose, "openpose")]
    [InlineData(ControlNetPreprocessor.OpenposeFace, "openpose_face")]
    [InlineData(ControlNetPreprocessor.OpenposeFull, "openpose_full")]
    [InlineData(ControlNetPreprocessor.MediapipeFace, "mediapipe_face")]
    [InlineData(ControlNetPreprocessor.NormalBae, "normal_bae")]
    [InlineData(ControlNetPreprocessor.Segmentation, "seg")]
    [InlineData(ControlNetPreprocessor.Shuffle, "shuffle")]
    [InlineData(ControlNetPreprocessor.Tile, "tile")]
    [InlineData(ControlNetPreprocessor.Inpaint, "inpaint")]
    [InlineData(ControlNetPreprocessor.Mlsd, "mlsd")]
    [InlineData(ControlNetPreprocessor.Scribble, "scribble")]
    [InlineData(ControlNetPreprocessor.Rembg, "rembg")]
    [InlineData(ControlNetPreprocessor.None, "none")]
    public void WhenConvertingControlNetPreprocessorToApiStringThenReturnsCorrectValue(
        ControlNetPreprocessor preprocessor,
        string expected)
    {
        // Act
        var result = preprocessor.ToApiString();

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Invalid Parsing Tests

    [Fact]
    public void WhenParsingInvalidAirEcosystemThenReturnsFalse()
    {
        // Act
        var success = EnumExtensions.TryParseFromApiString<AirEcosystem>("invalid_ecosystem", out _);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void WhenParsingInvalidSchedulerThenReturnsFalse()
    {
        // Act
        var success = EnumExtensions.TryParseFromApiString<Scheduler>("invalid_scheduler", out _);

        // Assert
        Assert.False(success);
    }

    #endregion
}
