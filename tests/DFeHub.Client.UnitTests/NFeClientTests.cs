using System.Text.Json;
using DFeHub.Client.UnitTests.TestDoubles;
using DFeHub.Exceptions;
using DFeHub.Http;
using DFeHub.Providers.WebMania;
using DFeHub.Providers.WebMania.NFe;

namespace DFeHub.Client.UnitTests;

public sealed class NFeClientTests
{
    private const string AccessKey =
        "35260712345678000190550010000001231000001234";

    [Fact]
    public async Task Issue_uses_api_v1_route_and_official_wire_format()
    {
        var (client, transport) = CreateClient(
            new NFeDocumentResponse { Status = "aprovado" });
        using var cancellation = new CancellationTokenSource();

        await client.IssueAsync(CreateIssueRequest(), cancellation.Token);

        Assert.Equal(WebManiaTransportKind.ApiV1, transport.TransportKind);
        Assert.Equal(HttpMethod.Post, transport.Method);
        Assert.Equal("1/nfe/emissao/", transport.Uri?.OriginalString);
        Assert.Equal("nfe", transport.Product);
        Assert.Equal("issue", transport.Operation);
        Assert.Equal(cancellation.Token, transport.CancellationToken);

        using var body = JsonDocument.Parse(Assert.IsType<string>(transport.Body));
        var root = body.RootElement;
        Assert.Equal("PEDIDO-123", root.GetProperty("ID").GetString());
        Assert.Equal(1, root.GetProperty("operacao").GetInt32());
        Assert.Equal(1, root.GetProperty("modelo").GetInt32());
        Assert.Equal(2, root.GetProperty("ambiente").GetInt32());
        Assert.Equal(
            "2026-07-18 14:30:00",
            root.GetProperty("data_emissao").GetString());
        Assert.Equal(
            "15.90",
            root.GetProperty("produtos")[0]
                .GetProperty("subtotal")
                .GetString());
        Assert.Equal(
            "31.80",
            root.GetProperty("pedido").GetProperty("total").GetString());
    }

    [Fact]
    public async Task Query_by_access_key_sends_get_body_and_deserializes_result()
    {
        var expected = new NFeDocumentResponse
        {
            Uuid = Guid.Parse("f7b16d40-0fe1-48f2-a174-5ca75a949adc"),
            Status = "aprovado",
            AccessKey = AccessKey,
        };
        var (client, transport) = CreateClient(expected);

        var result = await client.QueryByAccessKeyAsync(AccessKey);

        Assert.Same(expected, result);
        Assert.Equal(HttpMethod.Get, transport.Method);
        Assert.Equal("1/nfe/consulta/", transport.Uri?.OriginalString);
        Assert.Equal("""{"chave":"35260712345678000190550010000001231000001234"}""",
            transport.Body);
    }

    [Fact]
    public async Task Query_by_order_id_preserves_multiple_documents()
    {
        NFeDocumentResponse[] expected =
        [
            new() { Status = "aprovado", Number = "123" },
            new() { Status = "cancelado", Number = "124" },
        ];
        var (client, transport) = CreateClient(expected);

        var result = await client.QueryByOrderIdAsync("PEDIDO-123");

        Assert.Equal(expected, result);
        Assert.Equal(HttpMethod.Get, transport.Method);
        Assert.Equal("""{"ID":"PEDIDO-123"}""", transport.Body);
    }

    [Fact]
    public async Task Cancellation_uses_put_and_provider_reason_name()
    {
        var (client, transport) = CreateClient(
            new NFeCancellationResponse { Status = "cancelado" });

        await client.CancelAsync(
            new NFeCancellationRequest
            {
                AccessKey = AccessKey,
                Reason = "Pedido cancelado pelo cliente.",
            });

        Assert.Equal(HttpMethod.Put, transport.Method);
        Assert.Equal("1/nfe/cancelar/", transport.Uri?.OriginalString);
        using var body = JsonDocument.Parse(Assert.IsType<string>(transport.Body));
        Assert.Equal(
            "Pedido cancelado pelo cliente.",
            body.RootElement.GetProperty("motivo").GetString());
    }

    [Fact]
    public async Task Recipient_manifestation_serializes_event_code_as_string()
    {
        var (client, transport) = CreateClient(
            new NFeEventResponse { Status = "processando" });

        await client.ManifestRecipientAsync(
            new NFeRecipientManifestationRequest
            {
                AccessKey = AccessKey,
                Environment = NFeEnvironment.Certification,
                Manifestation =
                    NFeRecipientManifestation.OperationConfirmation,
            });

        Assert.Equal(HttpMethod.Post, transport.Method);
        Assert.Equal("1/nfe/manifesta/", transport.Uri?.OriginalString);
        using var body = JsonDocument.Parse(Assert.IsType<string>(transport.Body));
        Assert.Equal(
            "210200",
            body.RootElement.GetProperty("evento").GetString());
        Assert.False(body.RootElement.TryGetProperty("manifestation", out _));
    }

    [Fact]
    public async Task Status_requests_have_no_body()
    {
        var (client, transport) = CreateClient(
            new NFeSefazStatusResponse { Status = "online" });

        var result = await client.GetSefazStatusAsync();

        Assert.Equal("online", result.Status);
        Assert.Equal(HttpMethod.Get, transport.Method);
        Assert.Equal("1/nfe/sefaz/", transport.Uri?.OriginalString);
        Assert.Null(transport.Body);
    }

    [Fact]
    public async Task Download_accepts_only_authenticated_official_https_hosts()
    {
        var (client, transport) = CreateClient(new object());
        var url = new Uri(
            "https://nfe.webmania.com.br/xml/2026/07/documento.xml");

        await using var file = await client.DownloadAsync(url);

        Assert.Equal(HttpMethod.Get, transport.Method);
        Assert.Equal(url, transport.Uri);
        Assert.Equal("download", transport.Operation);
        Assert.Equal(3, file.Length);
    }

    [Theory]
    [InlineData("http://nfe.webmania.com.br/documento.xml")]
    [InlineData("https://webmania.com.br.evil.test/documento.xml")]
    [InlineData("https://example.test/documento.xml")]
    public async Task Download_rejects_untrusted_urls(string url)
    {
        var (client, _) = CreateClient(new object());

        var exception = await Assert.ThrowsAsync<WebManiaValidationException>(
            () => client.DownloadAsync(new Uri(url)));

        Assert.Contains(
            exception.Failures,
            failure => failure.Code == "NFE_DOWNLOAD_HOST");
    }

    [Fact]
    public async Task Invalid_request_is_rejected_before_transport()
    {
        var (client, transport) = CreateClient(
            new NFeEventResponse { Status = "unused" });

        var exception = await Assert.ThrowsAsync<WebManiaValidationException>(
            () => client.IssueCorrectionLetterAsync(
                new NFeCorrectionLetterRequest
                {
                    AccessKey = "123",
                    Correction = "curta",
                    Environment = NFeEnvironment.Certification,
                }));

        Assert.Equal(2, exception.Failures.Count);
        Assert.Null(transport.Method);
    }

    [Fact]
    public async Task Operation_not_performed_requires_justification()
    {
        var (client, _) = CreateClient(
            new NFeEventResponse { Status = "unused" });

        var exception = await Assert.ThrowsAsync<WebManiaValidationException>(
            () => client.ManifestRecipientAsync(
                new NFeRecipientManifestationRequest
                {
                    AccessKey = AccessKey,
                    Environment = NFeEnvironment.Certification,
                    Manifestation =
                        NFeRecipientManifestation.OperationNotPerformed,
                }));

        Assert.Contains(
            exception.Failures,
            failure => failure.PropertyPath == "Justification");
    }

    private static (
        NFeClient Client,
        RecordingNFeTransport Transport) CreateClient(object response)
    {
        var transport = new RecordingNFeTransport { Response = response };
        var options = new StaticOptionsMonitor<WebManiaOptions>(new());
        return (new NFeClient(options, transport), transport);
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
