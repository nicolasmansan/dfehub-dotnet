using System.Text.Json;
using DFeHub.Providers.WebMania.NFe;

namespace DFeHub.Client.UnitTests;

public sealed class NFeJsonConverterTests
{
    [Fact]
    public void Decimal_converter_reads_number_and_string_and_writes_string()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NFeDecimalStringConverter());

        Assert.Equal(12.34m, JsonSerializer.Deserialize<decimal>("12.34", options));
        Assert.Equal(
            12.34m,
            JsonSerializer.Deserialize<decimal>("\"12.34\"", options));
        Assert.Equal("\"12.34\"", JsonSerializer.Serialize(12.34m, options));
    }

    [Theory]
    [InlineData("true")]
    [InlineData("\"not-a-number\"")]
    public void Decimal_converter_rejects_invalid_values(string json)
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NFeDecimalStringConverter());

        Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<decimal>(json, options));
    }

    [Fact]
    public void Date_converter_reads_wire_and_iso_formats_and_writes_wire_format()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NFeDateTimeOffsetConverter());

        var wire = JsonSerializer.Deserialize<DateTimeOffset>(
            "\"2026-07-18 14:30:00\"",
            options);
        var iso = JsonSerializer.Deserialize<DateTimeOffset>(
            "\"2026-07-18T14:30:00-03:00\"",
            options);

        Assert.Equal(2026, wire.Year);
        Assert.Equal(TimeSpan.FromHours(-3), iso.Offset);
        Assert.Equal(
            "\"2026-07-18 14:30:00\"",
            JsonSerializer.Serialize(iso, options));
    }

    [Fact]
    public void Date_converter_rejects_invalid_value()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NFeDateTimeOffsetConverter());

        Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<DateTimeOffset>(
                "\"not-a-date\"",
                options));
    }
}
