using System.Text.Json;
using DFeHub.Client.UnitTests.TestDoubles;
using DFeHub.Exceptions;
using DFeHub.Http;
using DFeHub.Providers.WebMania;
using DFeHub.Providers.WebMania.NFSe;

namespace DFeHub.Client.UnitTests;

public sealed class NFSeClientTests
{
    private static readonly Guid InvoiceUuid =
        Guid.Parse("127072a3-34c5-48c7-a160-f45816cbe532");

    [Fact]
    public async Task Issue_uses_api_v2_route_and_official_wire_format()
    {
        var (client, transport) = CreateClient(Document());
        using var cancellation = new CancellationTokenSource();

        await client.IssueAsync(CreateIssueRequest(), cancellation.Token);

        Assert.Equal(WebManiaTransportKind.ApiV2, transport.TransportKind);
        Assert.Equal(HttpMethod.Post, transport.Method);
        Assert.Equal("2/nfse/emissao", transport.Uri?.OriginalString);
        Assert.Equal("nfse", transport.Product);
        Assert.Equal("issue", transport.Operation);
        Assert.Equal(cancellation.Token, transport.CancellationToken);

        using var body = JsonDocument.Parse(Assert.IsType<string>(transport.Body));
        var root = body.RootElement;
        Assert.Equal("PEDIDO-123", root.GetProperty("ID").GetString());
        Assert.Equal(2, root.GetProperty("ambiente").GetInt32());
        Assert.Equal(
            "2026-07-20 14:30:00",
            root.GetProperty("data_agendamento").GetString());
        Assert.Equal(
            "150.00",
            root.GetProperty("rps")[0]
                .GetProperty("servico")
                .GetProperty("valor_servicos")
                .GetString());
        Assert.Equal(
            "REF000000",
            root.GetProperty("rps")[0]
                .GetProperty("servico")
                .GetProperty("classe_imposto")
                .GetString());
    }

    [Fact]
    public async Task Status_and_query_are_get_requests_without_bodies()
    {
        var (client, transport) = CreateClient(
            new NFSeProviderStatusResponse
            {
                Status = true,
                Model = "abrasf",
                Version = "2.04",
            });

        var status = await client.GetMunicipalityStatusAsync();

        Assert.True(status.Status);
        Assert.Equal(HttpMethod.Get, transport.Method);
        Assert.Equal("2/nfse/status", transport.Uri?.OriginalString);
        Assert.Null(transport.Body);

        transport.Response = Document();
        var document = await client.QueryByUuidAsync(InvoiceUuid);

        Assert.Equal("aprovado", document.Status);
        Assert.Equal(HttpMethod.Get, transport.Method);
        Assert.Equal(
            $"2/nfse/consulta/{InvoiceUuid:D}",
            transport.Uri?.OriginalString);
        Assert.Null(transport.Body);
    }

    [Fact]
    public async Task Fiscal_mutations_dispatch_to_the_official_routes()
    {
        var (client, transport) = CreateClient(Document());

        await client.CancelAsync(
            new NFSeCancellationRequest
            {
                Uuid = InvoiceUuid,
                Reason = (NFSeCancellationReason)1,
            });
        AssertCall(transport, HttpMethod.Put, "2/nfse/cancelar");

        transport.Response = Document();
        await client.SubstituteAsync(
            new NFSeSubstitutionRequest
            {
                Environment = NFSeEnvironment.Certification,
                VerificationCode = "ABC-123",
                Reason = (NFSeCancellationReason)1,
                Rps = CreateRps(),
            });
        AssertCall(transport, HttpMethod.Post, "2/nfse/substituir");

        transport.Response = Document();
        await client.ManifestAsync(
            new NFSeManifestationRequest
            {
                Environment = NFSeEnvironment.Certification,
                Uuid = InvoiceUuid,
                ManifestingParty = (NFSeManifestingParty)1,
                Event = (NFSeManifestationEvent)1,
            });
        AssertCall(transport, HttpMethod.Post, "2/nfse/manifestar");
    }

    [Fact]
    public async Task Download_accepts_only_authenticated_official_https_hosts()
    {
        var (client, transport) = CreateClient(new object());
        var url = new Uri(
            $"https://api.webmania.com.br/xmlnfse/{InvoiceUuid:D}");

        await using var file = await client.DownloadAsync(url);

        Assert.Equal(WebManiaTransportKind.ApiV2, transport.TransportKind);
        Assert.Equal(HttpMethod.Get, transport.Method);
        Assert.Equal(url, transport.Uri);
        Assert.Equal("download", transport.Operation);
        Assert.Equal(3, file.Length);

        var exception = await Assert.ThrowsAsync<WebManiaValidationException>(
            () => client.DownloadAsync(
                new Uri("https://webmania.com.br.evil.test/documento.xml")));
        Assert.Contains(
            exception.Failures,
            failure => failure.Code == "NFSE_DOWNLOAD_HOST");
    }

    [Fact]
    public async Task Invalid_request_is_rejected_before_transport()
    {
        var (client, transport) = CreateClient(Document());

        var exception = await Assert.ThrowsAsync<WebManiaValidationException>(
            () => client.IssueAsync(
                CreateIssueRequest() with
                {
                    OrderId = string.Empty,
                    Rps = [],
                }));

        Assert.NotEmpty(exception.Failures);
        Assert.Null(transport.Method);
    }

    private static (
        NFSeClient Client,
        RecordingNFSeTransport Transport) CreateClient(object response)
    {
        var transport = new RecordingNFSeTransport { Response = response };
        var options = new StaticOptionsMonitor<WebManiaOptions>(new());
        return (new NFSeClient(options, transport), transport);
    }

    private static NFSeIssueRequest CreateIssueRequest()
    {
        return new NFSeIssueRequest
        {
            OrderId = "PEDIDO-123",
            Environment = NFSeEnvironment.Certification,
            NotificationUrl =
                new Uri("https://example.test/webhooks/nfse"),
            ScheduledAt = new DateTime(
                2026,
                7,
                20,
                14,
                30,
                0,
                DateTimeKind.Unspecified),
            Rps = [CreateRps()],
        };
    }

    private static NFSeRps CreateRps()
    {
        return new NFSeRps
        {
            Service = new NFSeService
            {
                Discrimination = "Licenciamento de software",
                ServiceValue = 150.00m,
                TaxClass = "REF000000",
            },
            Taker = new NFSeParty
            {
                Cnpj = "12345678000190",
                CorporateName = "Empresa de Homologação",
                Address = "Rua Fiscal",
                Number = "100",
                District = "Centro",
                City = "Curitiba",
                State = "PR",
                PostalCode = "80000000",
            },
        };
    }

    private static NFSeDocumentResponse Document()
    {
        return new NFSeDocumentResponse
        {
            Uuid = InvoiceUuid,
            Model = "nfse",
            Status = "aprovado",
        };
    }

    private static void AssertCall(
        RecordingNFSeTransport transport,
        HttpMethod method,
        string route)
    {
        Assert.Equal(method, transport.Method);
        Assert.Equal(route, transport.Uri?.OriginalString);
    }
}
