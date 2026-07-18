using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DFeHub.Providers.WebMania.NFSe;

internal sealed class NFSeDecimalStringConverter : JsonConverter<decimal>
{
    public override decimal Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number &&
            reader.TryGetDecimal(out var number))
        {
            return number;
        }

        if (reader.TokenType == JsonTokenType.String &&
            decimal.TryParse(
                reader.GetString(),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out number))
        {
            return number;
        }

        throw new JsonException(
            "Expected an NFS-e decimal number or numeric string.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        decimal value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
    }
}

internal sealed class NFSeScheduledDateTimeConverter
    : JsonConverter<DateTime>
{
    private const string WireFormat = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (DateTime.TryParseExact(
                value,
                WireFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var result))
        {
            return result;
        }

        throw new JsonException(
            "Expected an NFS-e schedule in yyyy-MM-dd HH:mm:ss format.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTime value,
        JsonSerializerOptions options)
    {
        if (value.Kind != DateTimeKind.Unspecified ||
            value.Ticks % TimeSpan.TicksPerMinute != 0)
        {
            throw new JsonException(
                "An NFS-e schedule must be an unspecified wall-clock time " +
                "with zero seconds.");
        }

        writer.WriteStringValue(
            value.ToString(WireFormat, CultureInfo.InvariantCulture));
    }
}

internal sealed class NFSeDateOnlyConverter : JsonConverter<DateOnly>
{
    private static readonly string[] WireFormats =
        ["yyyy-MM-dd", "dd/MM/yyyy"];

    public override DateOnly Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (DateOnly.TryParseExact(
                value,
                WireFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var result))
        {
            return result;
        }

        throw new JsonException("Expected an NFS-e calendar date.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateOnly value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(
            value.ToString(WireFormats[0], CultureInfo.InvariantCulture));
    }
}

internal sealed class NFSeStringOrNumberConverter : JsonConverter<string>
{
    public override string Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString() ??
                throw new JsonException("Expected a non-null string.");
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            using var value = JsonDocument.ParseValue(ref reader);
            return value.RootElement.GetRawText();
        }

        throw new JsonException("Expected a string or number.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        string value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}

internal sealed class NFSeStringOrIntValueConverter
    : JsonConverter<NFSeStringOrIntValue>
{
    public override NFSeStringOrIntValue Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number when reader.TryGetInt32(out var value) =>
                NFSeStringOrIntValue.FromInt32(value),
            JsonTokenType.String =>
                NFSeStringOrIntValue.FromString(
                    reader.GetString() ??
                    throw new JsonException("Expected a non-null string.")),
            _ => throw new JsonException(
                "Expected a provider string or integer code."),
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        NFSeStringOrIntValue value,
        JsonSerializerOptions options)
    {
        if (value.IntegerValue is { } integerValue)
        {
            writer.WriteNumberValue(integerValue);
            return;
        }

        if (value.StringValue is { } stringValue)
        {
            writer.WriteStringValue(stringValue);
            return;
        }

        throw new JsonException("The provider code has no value.");
    }
}

internal sealed class NFSeRealEstateAddressConverter
    : JsonConverter<NFSeRealEstateAddress>
{
    public override NFSeRealEstateAddress Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new NFSeRealEstateAddress(
                reader.GetString() ??
                throw new JsonException("Expected a non-null address."));
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            var value = JsonSerializer.Deserialize(
                ref reader,
                NFSeJsonSerializerContext.Default.NFSeStructuredAddress);
            return new NFSeRealEstateAddress(
                value ??
                throw new JsonException("Expected a structured address."));
        }

        throw new JsonException(
            "Expected a direct or structured real-estate address.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        NFSeRealEstateAddress value,
        JsonSerializerOptions options)
    {
        if (value.DirectAddress is not null &&
            value.StructuredAddress is not null)
        {
            throw new JsonException(
                "A real-estate address cannot contain both wire shapes.");
        }

        if (value.DirectAddress is { } directAddress)
        {
            writer.WriteStringValue(directAddress);
            return;
        }

        if (value.StructuredAddress is { } structuredAddress)
        {
            JsonSerializer.Serialize(
                writer,
                structuredAddress,
                NFSeJsonSerializerContext.Default.NFSeStructuredAddress);
            return;
        }

        writer.WriteNullValue();
    }
}
