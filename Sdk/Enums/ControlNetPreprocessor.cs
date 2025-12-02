namespace CivitaiSharp.Sdk.Enums;

/// <summary>
/// ControlNet preprocessor types for image guidance.
/// </summary>
/// <remarks>
/// API string mappings are defined in <see cref="Extensions.SdkApiStringRegistry"/>.
/// </remarks>
public enum ControlNetPreprocessor
{
    /// <summary>
    /// Canny edge detection preprocessor.
    /// Maps to API value "canny".
    /// </summary>
    Canny,

    /// <summary>
    /// Depth estimation preprocessor.
    /// Maps to API value "depth".
    /// </summary>
    Depth,

    /// <summary>
    /// Depth (LeReS) estimation preprocessor.
    /// Maps to API value "depth_leres".
    /// </summary>
    DepthLeres,

    /// <summary>
    /// Depth (MiDaS) estimation preprocessor.
    /// Maps to API value "depth_midas".
    /// </summary>
    DepthMidas,

    /// <summary>
    /// Depth (Zoe) estimation preprocessor.
    /// Maps to API value "depth_zoe".
    /// </summary>
    DepthZoe,

    /// <summary>
    /// HED (Holistically-Nested Edge Detection) soft edge preprocessor.
    /// Maps to API value "softedge_hed".
    /// </summary>
    SoftEdgeHed,

    /// <summary>
    /// PiDiNet soft edge preprocessor.
    /// Maps to API value "softedge_pidinet".
    /// </summary>
    SoftEdgePidinet,

    /// <summary>
    /// Lineart preprocessor.
    /// Maps to API value "lineart".
    /// </summary>
    Lineart,

    /// <summary>
    /// Anime lineart preprocessor.
    /// Maps to API value "lineart_anime".
    /// </summary>
    LineartAnime,

    /// <summary>
    /// OpenPose pose estimation preprocessor.
    /// Maps to API value "openpose".
    /// </summary>
    Openpose,

    /// <summary>
    /// OpenPose face preprocessor.
    /// Maps to API value "openpose_face".
    /// </summary>
    OpenposeFace,

    /// <summary>
    /// OpenPose full body preprocessor.
    /// Maps to API value "openpose_full".
    /// </summary>
    OpenposeFull,

    /// <summary>
    /// MediaPipe face mesh preprocessor.
    /// Maps to API value "mediapipe_face".
    /// </summary>
    MediapipeFace,

    /// <summary>
    /// Normal map preprocessor.
    /// Maps to API value "normal_bae".
    /// </summary>
    NormalBae,

    /// <summary>
    /// Segmentation preprocessor.
    /// Maps to API value "seg".
    /// </summary>
    Segmentation,

    /// <summary>
    /// Shuffle preprocessor for reference-based generation.
    /// Maps to API value "shuffle".
    /// </summary>
    Shuffle,

    /// <summary>
    /// Tile preprocessor for upscaling and detail enhancement.
    /// Maps to API value "tile".
    /// </summary>
    Tile,

    /// <summary>
    /// Inpaint preprocessor.
    /// Maps to API value "inpaint".
    /// </summary>
    Inpaint,

    /// <summary>
    /// MLSD (M-LSD) line segment detection preprocessor.
    /// Maps to API value "mlsd".
    /// </summary>
    Mlsd,

    /// <summary>
    /// Scribble preprocessor for hand-drawn input.
    /// Maps to API value "scribble".
    /// </summary>
    Scribble,

    /// <summary>
    /// Background removal preprocessor for subject isolation.
    /// Maps to API value "rembg".
    /// </summary>
    Rembg,

    /// <summary>
    /// No preprocessing - use the image as-is.
    /// Maps to API value "none".
    /// </summary>
    None
}
