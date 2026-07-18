using DFeHub.Client.UnitTests.TestDoubles;
using DFeHub.Providers.WebMania;
using DFeHub.Providers.WebMania.NFe;

namespace DFeHub.Client.UnitTests;

public sealed class NFeClientOperationTests
{
    private const string AccessKey =
        "35260712345678000190550010000001231000001234";

    [Fact]
    public async Task Remaining_document_operations_dispatch_to_official_routes()
    {
        var (client, transport) = CreateClient();

        transport.Response = Document();
        await client.IssueReturnAsync(
            new NFeReturnRequest
            {
                AccessKey = AccessKey,
                OperationNature = "Devolução de mercadoria",
                Environment = NFeEnvironment.Certification,
                CfopCode = "5202",
            });
        AssertCall(transport, HttpMethod.Post, "1/nfe/devolucao/");

        transport.Response = Document();
        await client.IssueAdjustmentAsync(
            new NFeAdjustmentRequest
            {
                Operation = NFeOperation.Outbound,
                OperationNature = "Ajuste de ICMS",
                CfopCode = "5949",
                IcmsValue = 10,
                Environment = NFeEnvironment.Certification,
                Customer = Customer(),
            });
        AssertCall(transport, HttpMethod.Post, "1/nfe/ajuste/");

        transport.Response = Document();
        await client.IssueComplementAsync(
            new NFeComplementRequest
            {
                AccessKey = AccessKey,
                Operation = NFeOperation.Outbound,
                OperationNature = "Complemento de ICMS",
                Environment = NFeEnvironment.Certification,
                Customer = Customer(),
                Taxes = new NFeComplementTaxes { CfopCode = "5949" },
            });
        AssertCall(transport, HttpMethod.Post, "1/nfe/complementar/");

        transport.Response = Event();
        await client.IssueCorrectionLetterAsync(
            new NFeCorrectionLetterRequest
            {
                AccessKey = AccessKey,
                Correction = "Corrigir o texto complementar da operação.",
                Environment = NFeEnvironment.Certification,
            });
        AssertCall(transport, HttpMethod.Post, "1/nfe/cartacorrecao/");

        transport.Response = new NFeNumberInvalidationResponse
        {
            Xml = new Uri("https://nfe.webmania.com.br/inutilizacao.xml"),
        };
        await client.InvalidateNumbersAsync(
            new NFeNumberInvalidationRequest
            {
                Sequence = "123-125",
                Reason = "Numeração não utilizada no sistema.",
                Environment = NFeEnvironment.Certification,
                Series = "1",
                Model = NFeModel.NFe,
            });
        AssertCall(transport, HttpMethod.Put, "1/nfe/inutilizar/");
    }

    [Fact]
    public async Task Fiscal_event_operations_dispatch_to_official_routes()
    {
        var (client, transport) = CreateClient();

        transport.Response = Event();
        await client.RegisterIbsCbsEventAsync(
            new NFeIbsCbsEventRequest
            {
                AccessKey = AccessKey,
                Environment = NFeEnvironment.Certification,
                EventCode = NFeIbsCbsEventCodes.FullPayment,
            });
        AssertCall(transport, HttpMethod.Post, "1/nfe/evento-ibs-cbs/");

        transport.Response = Event();
        await client.CancelIbsCbsEventAsync(
            new NFeIbsCbsEventCancellationRequest
            {
                Uuid = Guid.Parse("3d86af2c-25ef-4a65-a965-9edb3fd1748d"),
            });
        AssertCall(
            transport,
            HttpMethod.Put,
            "1/nfe/evento-ibs-cbs/cancelar/");

        transport.Response = Event();
        await client.RegisterFinancialReconciliationAsync(
            new NFeFinancialReconciliationRequest
            {
                AccessKey = AccessKey,
                Payment = Payment(),
            });
        AssertCall(transport, HttpMethod.Post, "1/nfe/conciliacao/");

        transport.Response = Event();
        await client.CancelFinancialReconciliationAsync(
            new NFeFinancialReconciliationCancellationRequest
            {
                Uuid = Guid.Parse("49db87e0-1c0e-4435-a11a-cd1717a587f9"),
            });
        AssertCall(
            transport,
            HttpMethod.Put,
            "1/nfe/conciliacao/cancelar/");
    }

    [Fact]
    public async Task Query_and_management_operations_dispatch_to_official_routes()
    {
        var (client, transport) = CreateClient();

        transport.Response = new NFeCertificateStatusResponse
        {
            DaysUntilExpiration = 90,
        };
        await client.GetCertificateStatusAsync();
        AssertCall(transport, HttpMethod.Get, "1/nfe/certificado/");

        transport.Response = Document();
        await client.QueryByUuidAsync(
            Guid.Parse("f7b16d40-0fe1-48f2-a174-5ca75a949adc"));
        AssertCall(transport, HttpMethod.Get, "1/nfe/consulta/");

        transport.Response = TaxClass();
        await client.UpsertTaxClassAsync(TaxClassRequest());
        AssertCall(transport, HttpMethod.Post, "1/nfe/classe-imposto/");

        transport.Response = new[] { TaxClass() };
        await client.ListTaxClassesAsync();
        AssertCall(transport, HttpMethod.Get, "1/nfe/classe-imposto/");

        transport.Response =
            new[]
            {
                new NFeTaxClassDeleteResponse
                {
                    Reference = "PADRAO",
                    Message = "Excluída",
                },
            };
        await client.DeleteTaxClassesAsync(
            new NFeTaxClassDeleteRequest { References = ["PADRAO"] });
        AssertCall(transport, HttpMethod.Delete, "1/nfe/classe-imposto/");

        transport.Response = new NFeReportResponse
        {
            Status = "processando",
            StartDate = new DateOnly(2026, 7, 1),
            EndDate = new DateOnly(2026, 7, 18),
            Model = "nfe",
            Report = "xml",
        };
        await client.ExportReportAsync(
            new NFeReportRequest
            {
                StartDate = new DateOnly(2026, 7, 1),
                EndDate = new DateOnly(2026, 7, 18),
                Model = "nfe",
                Report = "xml",
            });
        AssertCall(transport, HttpMethod.Post, "1/nfe/relatorios/");

        transport.Response =
            new NFeCompanyUpdateResponse { Success = "Atualizada" };
        await client.UpdateCompanyAsync(
            new NFeCompanyUpdateRequest { Email = "fiscal@example.test" });
        AssertCall(transport, HttpMethod.Post, "1/nfe/empresa/");
    }

    private static (
        NFeClient Client,
        RecordingNFeTransport Transport) CreateClient()
    {
        var transport = new RecordingNFeTransport();
        var options = new StaticOptionsMonitor<WebManiaOptions>(new());
        return (new NFeClient(options, transport), transport);
    }

    private static void AssertCall(
        RecordingNFeTransport transport,
        HttpMethod method,
        string route)
    {
        Assert.Equal(method, transport.Method);
        Assert.Equal(route, transport.Uri?.OriginalString);
    }

    private static NFeDocumentResponse Document()
    {
        return new NFeDocumentResponse { Status = "aprovado" };
    }

    private static NFeEventResponse Event()
    {
        return new NFeEventResponse { Status = "registrado" };
    }

    private static NFeCustomer Customer()
    {
        return new NFeCustomer
        {
            Cnpj = "12345678000190",
            CorporateName = "Empresa de Homologação",
            Address = "Rua Fiscal",
            Number = "1",
            District = "Centro",
            City = "Curitiba",
            State = "PR",
        };
    }

    private static NFeReconciliationPayment Payment()
    {
        return new NFeReconciliationPayment
        {
            PaymentIndicator = 0,
            PaymentMethod = "01",
            Value = 10,
            Date = new DateOnly(2026, 7, 18),
        };
    }

    private static NFeTaxClassUpsertRequest TaxClassRequest()
    {
        var contribution = new NFeTaxClassContribution
        {
            Scenario = "padrao",
            PersonType = "juridica",
            TaxSituation = "01",
        };
        return new NFeTaxClassUpsertRequest
        {
            Description = "Classe padrão",
            Icms =
            [
                new NFeTaxClassIcms
                {
                    TaxRegime = "normal",
                    Scenario = "padrao",
                    PersonType = "juridica",
                    CfopCode = "5102",
                    TaxSituation = "00",
                },
            ],
            Ipi = [contribution],
            Pis = [contribution],
            Cofins = [contribution],
        };
    }

    private static NFeTaxClassResponse TaxClass()
    {
        return new NFeTaxClassResponse
        {
            Reference = "PADRAO",
            Description = "Classe padrão",
        };
    }
}
