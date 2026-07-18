using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using DFeHub.Providers.WebMania.NFSe;
using DFeHub.Serialization;

namespace DFeHub.Client.ContractTests;

public sealed class NFSeContractTests
{
    [Fact]
    public void Official_endpoint_matrix_is_frozen()
    {
        var expected = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["StatusRoute"] = "2/nfse/status",
            ["IssueRoute"] = "2/nfse/emissao",
            ["QueryRoute"] = "2/nfse/consulta",
            ["CancellationRoute"] = "2/nfse/cancelar",
            ["SubstitutionRoute"] = "2/nfse/substituir",
            ["ManifestationRoute"] = "2/nfse/manifestar",
        };
        var actual = typeof(NFSeClient)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(field =>
                field.IsLiteral &&
                field.FieldType == typeof(string) &&
                field.Name.EndsWith("Route", StringComparison.Ordinal))
            .ToDictionary(
                field => field.Name,
                field => Assert.IsType<string>(field.GetRawConstantValue()),
                StringComparer.Ordinal);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void National_standard_issue_fixture_round_trips()
    {
        var expected = JsonNode.Parse(ReadFixture("issue-request-national.json"));
        var request = JsonSerializer.Deserialize(
            expected!.ToJsonString(),
            NFSeJsonSerializerContext.Default.NFSeIssueRequest);

        Assert.NotNull(request);
        Assert.Equal("PEDIDO-123", request.OrderId);
        Assert.Equal(NFSeEnvironment.Certification, request.Environment);
        Assert.Single(request.Rps);
        Assert.Equal(150.00m, request.Rps[0].Service.ServiceValue);
        Assert.Equal(
            "000001",
            request.Rps[0].Service.Taxes?.IbsCbs?.TaxClassification);

        var actual = JsonNode.Parse(
            JsonSerializer.Serialize(
                request,
                NFSeJsonSerializerContext.Default.NFSeIssueRequest));

        Assert.True(
            JsonNode.DeepEquals(expected, actual),
            $"Expected:{Environment.NewLine}{expected}{Environment.NewLine}" +
            $"Actual:{Environment.NewLine}{actual}");
    }

    [Fact]
    public void Provider_status_fixture_keeps_typed_capabilities()
    {
        var response = JsonSerializer.Deserialize(
            ReadFixture("status-response.json"),
            NFSeJsonSerializerContext.Default.NFSeProviderStatusResponse);

        Assert.NotNull(response);
        Assert.True(response.Status);
        Assert.Equal("abrasf", response.Model);
        Assert.Equal("2.04", response.Version);
        Assert.Equal(
            [NFSeEnvironment.Production, NFSeEnvironment.Certification],
            response.Environments);
        Assert.Contains("lote_rps", response.Issuance);
        Assert.Equal("01.05", Assert.Single(response.ServiceCodes).Code);
        Assert.Equal(
            "obrigatorio",
            response.AvailableParameters["servico.valor_servicos"]);
    }

    [Fact]
    public void Invoice_response_accepts_numeric_strings_and_additive_fields()
    {
        var response = JsonSerializer.Deserialize(
            ReadFixture("issue-response-invoice.json"),
            NFSeJsonSerializerContext.Default.NFSeDocumentResponse);

        Assert.NotNull(response);
        Assert.Equal("aprovado", response.Status);
        Assert.Equal("2000", response.RpsNumber);
        Assert.Equal(
            new DateTimeOffset(
                2026,
                7,
                18,
                12,
                0,
                0,
                TimeSpan.FromHours(-3)),
            response.UpdatedAt);
        Assert.NotNull(response.AdditionalData);
        Assert.Equal(
            "preservado",
            response.AdditionalData["campo_aditivo_futuro"].GetString());
    }

    [Fact]
    public void Query_response_accepts_the_published_document_shape()
    {
        var response = JsonSerializer.Deserialize(
            ReadFixture("query-response.json"),
            NFSeJsonSerializerContext.Default.NFSeDocumentResponse);

        Assert.NotNull(response);
        Assert.Equal("aprovado", response.Status);
        Assert.Equal("2000", response.RpsNumber);
        Assert.Equal(JsonValueKind.String, response.Log.ValueKind);
        Assert.Equal("api.webmania.com.br", response.Xml?.Host);
    }

    [Fact]
    public void Batch_response_preserves_every_nested_invoice()
    {
        var response = JsonSerializer.Deserialize(
            ReadFixture("issue-response-batch.json"),
            NFSeJsonSerializerContext.Default.NFSeDocumentResponse);

        Assert.NotNull(response);
        Assert.Equal("lote_rps", response.Model);
        Assert.Equal("2", response.RpsQuantity);
        Assert.Equal(2, response.Invoices?.Count);
        Assert.Equal("1001", response.Invoices?[0].Number);
        Assert.Equal("processando", response.Invoices?[1].Status);
    }

    [Fact]
    public void Official_authentication_error_fixture_uses_shared_contract()
    {
        var response = JsonSerializer.Deserialize(
            ReadFixture("error-response.json"),
            WebManiaJsonSerializerContext.Default.WebManiaErrorPayload);

        Assert.NotNull(response);
        Assert.Equal("Acesso restrito.", response.Msg);
    }

    private static string ReadFixture(string name)
    {
        return File.ReadAllText(
            Path.Combine(AppContext.BaseDirectory, "Fixtures", "NFSe", name));
    }
}
