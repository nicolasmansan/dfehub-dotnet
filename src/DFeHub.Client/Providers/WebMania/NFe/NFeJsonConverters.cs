using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DFeHub.Providers.WebMania.NFe;

internal sealed class NFeDecimalStringConverter : JsonConverter<decimal>
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
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out number))
        {
            return number;
        }

        throw new JsonException("Expected a decimal number or numeric string.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        decimal value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
    }
}

internal sealed class NFeDateTimeOffsetConverter
    : JsonConverter<DateTimeOffset>
{
    private const string WireFormat = "yyyy-MM-dd HH:mm:ss";

    public override DateTimeOffset Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (DateTimeOffset.TryParseExact(
                value,
                WireFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal,
                out var result) ||
            DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out result))
        {
            return result;
        }

        throw new JsonException("Expected a Webmania date and time.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(
            value.ToString(WireFormat, CultureInfo.InvariantCulture));
    }
}
