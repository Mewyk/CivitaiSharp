namespace CivitaiSharp.Core.Extensions;

using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using CivitaiSharp.Core.Models;

/// <summary>
/// Provides AOT-compatible, reflection-free mappings between enum values and their API string representations.
/// All mappings use frozen dictionaries for optimal runtime performance.
/// </summary>
/// <remarks>
/// <para>
/// This registry eliminates the need for reflection when converting enums to API strings,
/// making the library compatible with AOT compilation and trimming.
/// </para>
/// <para>
/// External libraries (like CivitaiSharp.Sdk) can register additional enum mappings using
/// <see cref="Register{TEnum}"/> at startup.
/// </para>
/// </remarks>
public static class ApiStringRegistry
{
    private static readonly FrozenDictionary<Type, object> CoreToApiStringMappings;
    private static readonly FrozenDictionary<Type, object> CoreFromApiStringMappings;

    private static readonly ConcurrentDictionary<Type, object> ExternalToApiStringMappings = new();
    private static readonly ConcurrentDictionary<Type, object> ExternalFromApiStringMappings = new();

    static ApiStringRegistry()
    {
        var toApiString = new Dictionary<Type, object>
        {
            [typeof(ModelSort)] = ModelSortMappings.ToApiString,
            [typeof(ImageSort)] = ImageSortMappings.ToApiString,
            [typeof(TimePeriod)] = TimePeriodMappings.ToApiString,
            [typeof(ModelType)] = ModelTypeMappings.ToApiString,
            [typeof(CommercialUsePermission)] = CommercialUsePermissionMappings.ToApiString,
            [typeof(ImageNsfwLevel)] = ImageNsfwLevelMappings.ToApiString,
            [typeof(ModelMode)] = ModelModeMappings.ToApiString,
            [typeof(MediaType)] = MediaTypeMappings.ToApiString,
            [typeof(Availability)] = AvailabilityMappings.ToApiString,
        };

        var fromApiString = new Dictionary<Type, object>
        {
            [typeof(ModelSort)] = ModelSortMappings.FromApiString,
            [typeof(ImageSort)] = ImageSortMappings.FromApiString,
            [typeof(TimePeriod)] = TimePeriodMappings.FromApiString,
            [typeof(ModelType)] = ModelTypeMappings.FromApiString,
            [typeof(CommercialUsePermission)] = CommercialUsePermissionMappings.FromApiString,
            [typeof(ImageNsfwLevel)] = ImageNsfwLevelMappings.FromApiString,
            [typeof(ModelMode)] = ModelModeMappings.FromApiString,
            [typeof(MediaType)] = MediaTypeMappings.FromApiString,
            [typeof(Availability)] = AvailabilityMappings.FromApiString,
        };

        CoreToApiStringMappings = toApiString.ToFrozenDictionary();
        CoreFromApiStringMappings = fromApiString.ToFrozenDictionary();
    }

    /// <summary>
    /// Registers an enum type with its API string mappings.
    /// This method is thread-safe and can be called at any time, though it's recommended
    /// to call it during application startup for best performance.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to register.</typeparam>
    /// <param name="toApiString">Dictionary mapping enum values to their API string representations.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="toApiString"/> is null.</exception>
    public static void Register<TEnum>(Dictionary<TEnum, string> toApiString)
        where TEnum : struct, Enum
    {
        ArgumentNullException.ThrowIfNull(toApiString);

        var frozenToApiString = toApiString.ToFrozenDictionary();
        var frozenFromApiString = CreateReverseLookup(frozenToApiString);

        ExternalToApiStringMappings[typeof(TEnum)] = frozenToApiString;
        ExternalFromApiStringMappings[typeof(TEnum)] = frozenFromApiString;
    }

    /// <summary>
    /// Converts an enum value to its API string representation (non-generic overload for AOT compatibility).
    /// This method uses pattern matching instead of reflection for full AOT support.
    /// </summary>
    /// <param name="value">The enum value to convert.</param>
    /// <returns>The API string representation, or the enum member name if no mapping exists.</returns>
    public static string GetApiString(Enum value)
    {
        ArgumentNullException.ThrowIfNull(value);

        // Pattern match on known core enum types
        return value switch
        {
            ModelSort modelSort => ToApiString(modelSort),
            ImageSort imageSort => ToApiString(imageSort),
            TimePeriod timePeriod => ToApiString(timePeriod),
            ModelType modelType => ToApiString(modelType),
            CommercialUsePermission commercialUse => ToApiString(commercialUse),
            ImageNsfwLevel nsfwLevel => ToApiString(nsfwLevel),
            ModelMode modelMode => ToApiString(modelMode),
            MediaType mediaType => ToApiString(mediaType),
            Availability availability => ToApiString(availability),
            _ => value.ToString()
        };
    }

    /// <summary>
    /// Converts an enum value to its API string representation.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="value">The enum value to convert.</param>
    /// <returns>The API string representation, or the enum member name if no mapping exists.</returns>
    public static string ToApiString<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        // Check external mappings first (for SDK enums)
        if (ExternalToApiStringMappings.TryGetValue(typeof(TEnum), out var externalMapping)
            && externalMapping is FrozenDictionary<TEnum, string> externalTypedMapping
            && externalTypedMapping.TryGetValue(value, out var externalApiString))
        {
            return externalApiString;
        }

        // Check core mappings
        if (CoreToApiStringMappings.TryGetValue(typeof(TEnum), out var coreMapping)
            && coreMapping is FrozenDictionary<TEnum, string> coreTypedMapping
            && coreTypedMapping.TryGetValue(value, out var coreApiString))
        {
            return coreApiString;
        }

        return value.ToString();
    }

    /// <summary>
    /// Attempts to parse an API string to the corresponding enum value.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="value">The API string value to parse.</param>
    /// <param name="result">When this method returns, contains the parsed enum value if successful.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    public static bool TryParseFromApiString<TEnum>(string? value, out TEnum result)
        where TEnum : struct, Enum
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        // Check external mappings first (for SDK enums)
        if (ExternalFromApiStringMappings.TryGetValue(typeof(TEnum), out var externalMapping)
            && externalMapping is FrozenDictionary<string, TEnum> externalTypedMapping
            && externalTypedMapping.TryGetValue(value, out result))
        {
            return true;
        }

        // Check core mappings
        if (CoreFromApiStringMappings.TryGetValue(typeof(TEnum), out var coreMapping)
            && coreMapping is FrozenDictionary<string, TEnum> coreTypedMapping
            && coreTypedMapping.TryGetValue(value, out result))
        {
            return true;
        }

        // Fallback to standard enum parsing
        return Enum.TryParse(value, ignoreCase: true, out result) && Enum.IsDefined(result);
    }

    /// <summary>
    /// Creates a case-insensitive reverse lookup dictionary from an enum-to-string mapping.
    /// Also adds the enum member names as fallback keys.
    /// </summary>
    internal static FrozenDictionary<string, TEnum> CreateReverseLookup<TEnum>(
        FrozenDictionary<TEnum, string> toApiStringMapping)
        where TEnum : struct, Enum
    {
        var lookup = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);

        foreach (var (enumValue, apiString) in toApiStringMapping)
        {
            lookup.TryAdd(apiString, enumValue);

            // Also add the enum member name as fallback
            var enumName = enumValue.ToString();
            if (!string.Equals(apiString, enumName, StringComparison.OrdinalIgnoreCase))
            {
                lookup.TryAdd(enumName, enumValue);
            }
        }

        return lookup.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Mappings for <see cref="ModelSort"/>.
    /// </summary>
    private static class ModelSortMappings
    {
        public static readonly FrozenDictionary<ModelSort, string> ToApiString =
            new Dictionary<ModelSort, string>
            {
                [ModelSort.HighestRated] = "Highest Rated",
                [ModelSort.MostDownloaded] = "Most Downloaded",
                [ModelSort.Newest] = "Newest",
            }.ToFrozenDictionary();

        public static readonly FrozenDictionary<string, ModelSort> FromApiString =
            CreateReverseLookup(ToApiString);
    }

    /// <summary>
    /// Mappings for <see cref="ImageSort"/>.
    /// </summary>
    private static class ImageSortMappings
    {
        public static readonly FrozenDictionary<ImageSort, string> ToApiString =
            new Dictionary<ImageSort, string>
            {
                [ImageSort.MostReactions] = "Most Reactions",
                [ImageSort.MostComments] = "Most Comments",
                [ImageSort.MostCollected] = "Most Collected",
                [ImageSort.Newest] = "Newest",
                [ImageSort.Oldest] = "Oldest",
                [ImageSort.Random] = "Random",
            }.ToFrozenDictionary();

        public static readonly FrozenDictionary<string, ImageSort> FromApiString =
            CreateReverseLookup(ToApiString);
    }

    /// <summary>
    /// Mappings for <see cref="TimePeriod"/>.
    /// </summary>
    private static class TimePeriodMappings
    {
        public static readonly FrozenDictionary<TimePeriod, string> ToApiString =
            new Dictionary<TimePeriod, string>
            {
                [TimePeriod.AllTime] = "AllTime",
                [TimePeriod.Year] = "Year",
                [TimePeriod.Month] = "Month",
                [TimePeriod.Week] = "Week",
                [TimePeriod.Day] = "Day",
            }.ToFrozenDictionary();

        public static readonly FrozenDictionary<string, TimePeriod> FromApiString =
            CreateReverseLookup(ToApiString);
    }

    /// <summary>
    /// Mappings for <see cref="ModelType"/>.
    /// </summary>
    private static class ModelTypeMappings
    {
        public static readonly FrozenDictionary<ModelType, string> ToApiString =
            new Dictionary<ModelType, string>
            {
                [ModelType.Checkpoint] = "Checkpoint",
                [ModelType.TextualInversion] = "TextualInversion",
                [ModelType.Hypernetwork] = "Hypernetwork",
                [ModelType.AestheticGradient] = "AestheticGradient",
                [ModelType.Lora] = "LORA",
                [ModelType.LoCon] = "LoCon",
                [ModelType.DoRa] = "DoRA",
                [ModelType.Controlnet] = "Controlnet",
                [ModelType.Poses] = "Poses",
                [ModelType.Upscaler] = "Upscaler",
                [ModelType.MotionModule] = "MotionModule",
                [ModelType.Vae] = "VAE",
                [ModelType.Wildcards] = "Wildcards",
                [ModelType.Workflows] = "Workflows",
                [ModelType.Other] = "Other",
            }.ToFrozenDictionary();

        public static readonly FrozenDictionary<string, ModelType> FromApiString =
            CreateReverseLookup(ToApiString);
    }

    /// <summary>
    /// Mappings for <see cref="CommercialUsePermission"/>.
    /// </summary>
    private static class CommercialUsePermissionMappings
    {
        public static readonly FrozenDictionary<CommercialUsePermission, string> ToApiString =
            new Dictionary<CommercialUsePermission, string>
            {
                [CommercialUsePermission.None] = "None",
                [CommercialUsePermission.Image] = "Image",
                [CommercialUsePermission.Rent] = "Rent",
                [CommercialUsePermission.RentCivit] = "RentCivit",
                [CommercialUsePermission.Sell] = "Sell",
            }.ToFrozenDictionary();

        public static readonly FrozenDictionary<string, CommercialUsePermission> FromApiString =
            CreateReverseLookup(ToApiString);
    }

    /// <summary>
    /// Mappings for <see cref="ImageNsfwLevel"/>.
    /// </summary>
    private static class ImageNsfwLevelMappings
    {
        public static readonly FrozenDictionary<ImageNsfwLevel, string> ToApiString =
            new Dictionary<ImageNsfwLevel, string>
            {
                [ImageNsfwLevel.None] = "None",
                [ImageNsfwLevel.Soft] = "Soft",
                [ImageNsfwLevel.Mature] = "Mature",
                [ImageNsfwLevel.Explicit] = "X",
            }.ToFrozenDictionary();

        public static readonly FrozenDictionary<string, ImageNsfwLevel> FromApiString =
            CreateReverseLookup(ToApiString);
    }

    /// <summary>
    /// Mappings for <see cref="ModelMode"/>.
    /// </summary>
    private static class ModelModeMappings
    {
        public static readonly FrozenDictionary<ModelMode, string> ToApiString =
            new Dictionary<ModelMode, string>
            {
                [ModelMode.Archived] = "Archived",
                [ModelMode.TakenDown] = "TakenDown",
            }.ToFrozenDictionary();

        public static readonly FrozenDictionary<string, ModelMode> FromApiString =
            CreateReverseLookup(ToApiString);
    }

    /// <summary>
    /// Mappings for <see cref="MediaType"/>.
    /// </summary>
    private static class MediaTypeMappings
    {
        public static readonly FrozenDictionary<MediaType, string> ToApiString =
            new Dictionary<MediaType, string>
            {
                [MediaType.Image] = "image",
                [MediaType.Video] = "video",
            }.ToFrozenDictionary();

        public static readonly FrozenDictionary<string, MediaType> FromApiString =
            CreateReverseLookup(ToApiString);
    }

    /// <summary>
    /// Mappings for <see cref="Availability"/>.
    /// </summary>
    private static class AvailabilityMappings
    {
        public static readonly FrozenDictionary<Availability, string> ToApiString =
            new Dictionary<Availability, string>
            {
                [Availability.Public] = "Public",
                [Availability.Private] = "Private",
                [Availability.Archived] = "Archived",
                [Availability.Unsearchable] = "Unsearchable",
            }.ToFrozenDictionary();

        public static readonly FrozenDictionary<string, Availability> FromApiString =
            CreateReverseLookup(ToApiString);
    }
}
