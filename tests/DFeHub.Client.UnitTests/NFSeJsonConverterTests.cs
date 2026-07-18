using System.Text.Json;
using DFeHub.Providers.WebMania.NFSe;

namespace DFeHub.Client.UnitTests;

public sealed class NFSeJsonConverterTests
{
    [Fact]
    public void Decimal_converter_reads_both_shapes_and_writes_string()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NFSeDecimalStringConverter());

        Assert.Equal(12.34m, JsonSerializer.Deserialize<decimal>("12.34", options));
        Assert.Equal(
            12.34m,
            JsonSerializer.Deserialize<decimal>("\"12.34\"", options));
        Assert.Equal("\"12.34\"", JsonSerializer.Serialize(12.34m, options));

        Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<decimal>("true", options));
        Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<decimal>(
                "\"not-a-number\"",
                options));
    }

    [Fact]
    public void Schedule_converter_preserves_wall_clock_and_minute_precision()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NFSeScheduledDateTimeConverter());

        var value = JsonSerializer.Deserialize<DateTime>(
            "\"2026-07-20 14:30:00\"",
            options);

        Assert.Equal(DateTimeKind.Unspecified, value.Kind);
        Assert.Equal(
            "\"2026-07-20 14:30:00\"",
            JsonSerializer.Serialize(value, options));
        Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<DateTime>(
                "\"not-a-date\"",
                options));
        Assert.Throws<JsonException>(
            () => JsonSerializer.Serialize(
                DateTime.SpecifyKind(value, DateTimeKind.Utc),
                options));
        Assert.Throws<JsonException>(
            () => JsonSerializer.Serialize(value.AddSeconds(1), options));
    }

    [Fact]
    public void Date_converter_reads_published_formats_and_writes_iso_date()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NFSeDateOnlyConverter());

        var iso = JsonSerializer.Deserialize<DateOnly>(
            "\"2026-07-18\"",
            options);
        var brazilian = JsonSerializer.Deserialize<DateOnly>(
            "\"18/07/2026\"",
            options);

        Assert.Equal(new DateOnly(2026, 7, 18), iso);
        Assert.Equal(iso, brazilian);
        Assert.Equal("\"2026-07-18\"", JsonSerializer.Serialize(iso, options));
        Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<DateOnly>(
                "\"not-a-date\"",
                options));
    }

    [Fact]
    public void String_or_number_converter_preserves_numeric_text()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NFSeStringOrNumberConverter());

        Assert.Equal(
            "001",
            JsonSerializer.Deserialize<string>("\"001\"", options));
        Assert.Equal(
            "12.50",
            JsonSerializer.Deserialize<string>("12.50", options));
        Assert.Equal("\"001\"", JsonSerializer.Serialize("001", options));
        Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<string>("true", options));
    }

    [Fact]
    public void Provider_code_converter_round_trips_integer_and_string()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NFSeStringOrIntValueConverter());

        NFSeStringOrIntValue integer = 7;
        NFSeStringOrIntValue text = "A";

        Assert.True(integer.IsInteger);
        Assert.Equal(7, integer.IntegerValue);
        Assert.Equal("7", integer.ToString());
        Assert.False(text.IsInteger);
        Assert.Equal("A", text.StringValue);
        Assert.Equal("A", text.ToString());
        Assert.Equal(
            integer,
            NFSeStringOrIntValue.FromInt32(7));
        Assert.Equal(
            text,
            NFSeStringOrIntValue.FromString("A"));
        Assert.Equal(
            integer,
            JsonSerializer.Deserialize<NFSeStringOrIntValue>("7", options));
        Assert.Equal(
            text,
            JsonSerializer.Deserialize<NFSeStringOrIntValue>("\"A\"", options));
        Assert.Equal("7", JsonSerializer.Serialize(integer, options));
        Assert.Equal("\"A\"", JsonSerializer.Serialize(text, options));
        Assert.Throws<ArgumentNullException>(
            () => new NFSeStringOrIntValue((string)null!));
        Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<NFSeStringOrIntValue>(
                "1.5",
                options));
        Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<NFSeStringOrIntValue>(
                "true",
                options));
        Assert.Throws<JsonException>(
            () => JsonSerializer.Serialize(
                default(NFSeStringOrIntValue),
                options));
    }

    [Fact]
    public void Real_estate_address_converter_round_trips_both_wire_shapes()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NFSeRealEstateAddressConverter());

        NFSeRealEstateAddress direct = "Av. Paulista";
        NFSeRealEstateAddress structured = new NFSeStructuredAddress
        {
            Street = "Av. Paulista",
            NationalAddress = new NFSeNationalAddress
            {
                IbgeCityCode = "3550308",
                PostalCode = "01310100",
            },
        };

        Assert.Equal(
            "\"Av. Paulista\"",
            JsonSerializer.Serialize(direct, options));
        using var structuredJson = JsonDocument.Parse(
            JsonSerializer.Serialize(structured, options));
        Assert.Equal(
            "Av. Paulista",
            structuredJson.RootElement.GetProperty("logradouro").GetString());

        var parsedDirect = JsonSerializer.Deserialize<NFSeRealEstateAddress>(
            "\"Av. Paulista\"",
            options);
        var parsedStructured =
            JsonSerializer.Deserialize<NFSeRealEstateAddress>(
                """
                {
                  "logradouro": "Av. Paulista",
                  "endereco_nacional": {
                    "codigo_municipio_ibge": "3550308",
                    "cep": "01310100"
                  }
                }
                """,
                options);

        Assert.Equal("Av. Paulista", parsedDirect?.DirectAddress);
        Assert.Equal(
            "3550308",
            parsedStructured?
                .StructuredAddress?
                .NationalAddress?
                .IbgeCityCode);
        Assert.Equal(
            "null",
            JsonSerializer.Serialize(new NFSeRealEstateAddress(), options));
        Assert.Throws<JsonException>(
            () => JsonSerializer.Serialize(
                new NFSeRealEstateAddress
                {
                    DirectAddress = "Av. Paulista",
                    StructuredAddress = new NFSeStructuredAddress(),
                },
                options));
        Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<NFSeRealEstateAddress>(
                "true",
                options));
    }
}
