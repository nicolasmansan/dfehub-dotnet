using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using DFeHub.Providers.WebMania.NFe;
using DFeHub.Serialization;

namespace DFeHub.Client.ContractTests;

public sealed class NFeContractTests
{
    [Fact]
    public void Official_endpoint_matrix_is_frozen()
    {
        var expected = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["IssueRoute"] = "1/nfe/emissao/",
            ["ReturnRoute"] = "1/nfe/devolucao/",
            ["AdjustmentRoute"] = "1/nfe/ajuste/",
            ["ComplementRoute"] = "1/nfe/complementar/",
            ["CorrectionLetterRoute"] = "1/nfe/cartacorrecao/",
            ["ManifestationRoute"] = "1/nfe/manifesta/",
            ["IbsCbsEventRoute"] = "1/nfe/evento-ibs-cbs/",
            ["IbsCbsEventCancellationRoute"] =
                "1/nfe/evento-ibs-cbs/cancelar/",
            ["ReconciliationRoute"] = "1/nfe/conciliacao/",
            ["ReconciliationCancellationRoute"] =
                "1/nfe/conciliacao/cancelar/",
            ["SefazRoute"] = "1/nfe/sefaz/",
            ["CertificateRoute"] = "1/nfe/certificado/",
            ["QueryRoute"] = "1/nfe/consulta/",
            ["CancellationRoute"] = "1/nfe/cancelar/",
            ["InvalidationRoute"] = "1/nfe/inutilizar/",
            ["TaxClassRoute"] = "1/nfe/classe-imposto/",
            ["ReportRoute"] = "1/nfe/relatorios/",
            ["CompanyRoute"] = "1/nfe/empresa/",
        };
        var actual = typeof(NFeClient)
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
    public void Issue_request_matches_official_fixture()
    {
        var actualJson = JsonSerializer.Serialize(
            CreateIssueRequest(),
            NFeJsonSerializerContext.Default.NFeIssueRequest);
        var expected = JsonNode.Parse(ReadFixture("issue-request.json"));
        var actual = JsonNode.Parse(actualJson);

        Assert.True(
            JsonNode.DeepEquals(expected, actual),
            $"Expected:{Environment.NewLine}{expected}{Environment.NewLine}" +
            $"Actual:{Environment.NewLine}{actual}");
    }

    [Fact]
    public void Issue_response_preserves_unknown_provider_fields()
    {
        var response = JsonSerializer.Deserialize(
            ReadFixture("issue-response.json"),
            NFeJsonSerializerContext.Default.NFeDocumentResponse);

        Assert.NotNull(response);
        Assert.Equal("aprovado", response.Status);
        Assert.Equal("Autorizado o uso da NF-e", response.Reason);
        Assert.Equal(
            "35260712345678000190550010000001231000001234",
            response.AccessKey);
        Assert.NotNull(response.AdditionalData);
        Assert.Equal(
            2,
            response.AdditionalData["campo_futuro"]
                .GetProperty("versao")
                .GetInt32());
    }

    [Fact]
    public void Error_fixture_matches_shared_api_error_contract()
    {
        var response = JsonSerializer.Deserialize(
            ReadFixture("error-response.json"),
            WebManiaJsonSerializerContext.Default.WebManiaErrorPayload);

        Assert.NotNull(response);
        Assert.Equal("Requisição inválida.", response.Error.GetString());
        Assert.Equal("O campo produtos é obrigatório.", response.Message);
        Assert.Equal("NFE_INVALID", response.Code);
    }

    [Fact]
    public void Ibs_cbs_event_catalog_contains_all_official_codes()
    {
        var codes = typeof(NFeIbsCbsEventCodes)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(field => Assert.IsType<string>(field.GetRawConstantValue()))
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "112110",
                "112120",
                "112130",
                "112140",
                "112150",
                "211110",
                "211120",
                "211124",
                "211128",
                "211130",
                "211140",
                "211150",
            ],
            codes);
    }

    [Fact]
    public void Nested_fiscal_models_keep_portuguese_wire_names()
    {
        var request = CreateIssueRequest() with
        {
            ReferencedEcfReceipts =
            [
                new NFeReferencedEcf
                {
                    Model = "2D",
                    EcfNumber = 1,
                    OperationCounter = 10,
                },
            ],
            ReferencedRuralInvoices =
            [
                new NFeReferencedRuralInvoice
                {
                    State = "PR",
                    Date = new DateOnly(2026, 7, 1),
                    Cpf = "12345678909",
                    StateRegistration = "123",
                    Model = "04",
                    Series = 1,
                    Number = 99,
                },
            ],
            Products =
            [
                CreateIssueRequest().Products[0] with
                {
                    Taxes = new NFeTaxes
                    {
                        Icms = new NFeIcmsTax
                        {
                            CfopCode = "5102",
                            TaxSituation = "00",
                            Rate = 18,
                        },
                    },
                },
            ],
        };

        var json = JsonSerializer.Serialize(
            request,
            NFeJsonSerializerContext.Default.NFeIssueRequest);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.Equal(
            "2D",
            root.GetProperty("ecf_referenciada")[0]
                .GetProperty("modelo")
                .GetString());
        var rural = root.GetProperty("nf_rural_referenciada")[0];
        Assert.Equal("2026-07-01", rural.GetProperty("data").GetString());
        Assert.Equal("04", rural.GetProperty("modelo").GetString());
        Assert.Equal(1, rural.GetProperty("serie").GetInt32());
        Assert.Equal(99, rural.GetProperty("numero").GetInt32());
        Assert.Equal(
            "18",
            root.GetProperty("produtos")[0]
                .GetProperty("impostos")
                .GetProperty("icms")
                .GetProperty("aliquota")
                .GetString());
    }

    private static string ReadFixture(string name)
    {
        return File.ReadAllText(
            Path.Combine(AppContext.BaseDirectory, "Fixtures", "NFe", name));
    }

    private static NFeIssueRequest CreateIssueRequest()
    {
        return new NFeIssueRequest
        {
            OrderId = "PEDIDO-123",
            Operation = NFeOperation.Outbound,
            OperationNature = "Venda de mercadoria",
            Model = NFeModel.NFe,
            Environment = NFeEnvironment.Certification,
            IssuedAt = new DateTimeOffset(
                2026,
                7,
                18,
                14,
                30,
                0,
                TimeSpan.FromHours(-3)),
            Customer = new NFeCustomer
            {
                Cpf = "12345678909",
                FullName = "Cliente de Homologação",
                Address = "Avenida das Nações",
                Number = "100",
                District = "Centro",
                City = "Curitiba",
                State = "PR",
                PostalCode = "80000000",
            },
            Products =
            [
                new NFeProduct
                {
                    Name = "Produto de teste",
                    Code = "SKU-1",
                    Ncm = "61091000",
                    Quantity = 2,
                    Unit = "UN",
                    Origin = 0,
                    UnitPrice = 15.90m,
                    Total = 31.80m,
                },
            ],
            Order = new NFeOrder
            {
                BuyerPresence = 1,
                FreightMode = 9,
                Total = 31.80m,
                PaymentTiming = [0],
                PaymentMethods = ["01"],
                PaymentValues = [31.80m],
            },
        };
    }
}
