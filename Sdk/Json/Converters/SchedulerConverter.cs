namespace CivitaiSharp.Sdk.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;

/// <summary>
/// AOT-compatible JSON converter for <see cref="Scheduler"/>.
/// </summary>
internal sealed class SchedulerConverter : JsonConverter<Scheduler>
{
    /// <inheritdoc />
    public override Scheduler Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(Scheduler)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "euler" => Scheduler.Euler,
            "euler_a" => Scheduler.EulerAncestral,
            "lms" => Scheduler.LinearMultistep,
            "heun" => Scheduler.Heun,
            "dpm_2" => Scheduler.DpmSolver2,
            "dpm_2_a" => Scheduler.DpmSolver2Ancestral,
            "dpmpp_2s_a" => Scheduler.DpmPlusPlus2SAncestral,
            "dpmpp_2m" => Scheduler.DpmPlusPlus2M,
            "dpmpp_sde" => Scheduler.DpmPlusPlusSde,
            "dpmpp_2m_sde" => Scheduler.DpmPlusPlus2MSde,
            "dpmpp_2m_sde_karras" => Scheduler.DpmPlusPlus2MSdeKarras,
            "dpmpp_3m_sde" => Scheduler.DpmPlusPlus3MSde,
            "dpmpp_3m_sde_karras" => Scheduler.DpmPlusPlus3MSdeKarras,
            "ddim" => Scheduler.Ddim,
            "plms" => Scheduler.Plms,
            "uni_pc" => Scheduler.UniPc,
            "uni_pc_bh2" => Scheduler.UniPcBh2,
            "ddpm" => Scheduler.Ddpm,
            "lcm" => Scheduler.Lcm,
            _ => throw new JsonException($"Unknown {nameof(Scheduler)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Scheduler value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            Scheduler.Euler => "euler",
            Scheduler.EulerAncestral => "euler_a",
            Scheduler.LinearMultistep => "lms",
            Scheduler.Heun => "heun",
            Scheduler.DpmSolver2 => "dpm_2",
            Scheduler.DpmSolver2Ancestral => "dpm_2_a",
            Scheduler.DpmPlusPlus2SAncestral => "dpmpp_2s_a",
            Scheduler.DpmPlusPlus2M => "dpmpp_2m",
            Scheduler.DpmPlusPlusSde => "dpmpp_sde",
            Scheduler.DpmPlusPlus2MSde => "dpmpp_2m_sde",
            Scheduler.DpmPlusPlus2MSdeKarras => "dpmpp_2m_sde_karras",
            Scheduler.DpmPlusPlus3MSde => "dpmpp_3m_sde",
            Scheduler.DpmPlusPlus3MSdeKarras => "dpmpp_3m_sde_karras",
            Scheduler.Ddim => "ddim",
            Scheduler.Plms => "plms",
            Scheduler.UniPc => "uni_pc",
            Scheduler.UniPcBh2 => "uni_pc_bh2",
            Scheduler.Ddpm => "ddpm",
            Scheduler.Lcm => "lcm",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(Scheduler)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}

/// <summary>
/// AOT-compatible JSON converter for nullable <see cref="Scheduler"/>.
/// Handles null values by writing JSON null and reading null tokens appropriately.
/// </summary>
internal sealed class NullableSchedulerConverter : JsonConverter<Scheduler?>
{
    private static readonly SchedulerConverter InnerConverter = new();

    /// <inheritdoc />
    public override Scheduler? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return InnerConverter.Read(ref reader, typeof(Scheduler), options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Scheduler? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            InnerConverter.Write(writer, value.Value, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
