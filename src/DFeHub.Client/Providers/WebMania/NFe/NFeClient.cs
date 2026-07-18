using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;
using DFeHub.Http;
using DFeHub.Providers.WebMania;
using Microsoft.Extensions.Options;

namespace DFeHub.Providers.WebMania.NFe;

internal sealed class NFeClient(
    IOptionsMonitor<WebManiaOptions> options,
    IWebManiaHttpTransport transport)
    : WebManiaProductClient(options, WebManiaCredentialGroup.ApiV1),
        INFeClient
{
    private const string Product = "nfe";
    private const string IssueRoute = "1/nfe/emissao/";
    private const string ReturnRoute = "1/nfe/devolucao/";
    private const string AdjustmentRoute = "1/nfe/ajuste/";
    private const string ComplementRoute = "1/nfe/complementar/";
    private const string CorrectionLetterRoute = "1/nfe/cartacorrecao/";
    private const string ManifestationRoute = "1/nfe/manifesta/";
    private const string IbsCbsEventRoute = "1/nfe/evento-ibs-cbs/";
    private const string IbsCbsEventCancellationRoute =
        "1/nfe/evento-ibs-cbs/cancelar/";
    private const string ReconciliationRoute = "1/nfe/conciliacao/";
    private const string ReconciliationCancellationRoute =
        "1/nfe/conciliacao/cancelar/";
    private const string SefazRoute = "1/nfe/sefaz/";
    private const string CertificateRoute = "1/nfe/certificado/";
    private const string QueryRoute = "1/nfe/consulta/";
    private const string CancellationRoute = "1/nfe/cancelar/";
    private const string InvalidationRoute = "1/nfe/inutilizar/";
    private const string TaxClassRoute = "1/nfe/classe-imposto/";
    private const string ReportRoute = "1/nfe/relatorios/";
    private const string CompanyRoute = "1/nfe/empresa/";

    public Task<NFeDocumentResponse> IssueAsync(
        NFeIssueRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            IssueRoute,
            request,
            NFeJsonSerializerContext.Default.NFeIssueRequest,
            NFeJsonSerializerContext.Default.NFeDocumentResponse,
            "issue",
            cancellationToken);
    }

    public Task<NFeDocumentResponse> IssueReturnAsync(
        NFeReturnRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            ReturnRoute,
            request,
            NFeJsonSerializerContext.Default.NFeReturnRequest,
            NFeJsonSerializerContext.Default.NFeDocumentResponse,
            "issue-return",
            cancellationToken);
    }

    public Task<NFeDocumentResponse> IssueAdjustmentAsync(
        NFeAdjustmentRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            AdjustmentRoute,
            request,
            NFeJsonSerializerContext.Default.NFeAdjustmentRequest,
            NFeJsonSerializerContext.Default.NFeDocumentResponse,
            "issue-adjustment",
            cancellationToken);
    }

    public Task<NFeDocumentResponse> IssueComplementAsync(
        NFeComplementRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            ComplementRoute,
            request,
            NFeJsonSerializerContext.Default.NFeComplementRequest,
            NFeJsonSerializerContext.Default.NFeDocumentResponse,
            "issue-complement",
            cancellationToken);
    }

    public Task<NFeEventResponse> IssueCorrectionLetterAsync(
        NFeCorrectionLetterRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            CorrectionLetterRoute,
            request,
            NFeJsonSerializerContext.Default.NFeCorrectionLetterRequest,
            NFeJsonSerializerContext.Default.NFeEventResponse,
            "correction-letter",
            cancellationToken);
    }

    public Task<NFeEventResponse> ManifestRecipientAsync(
        NFeRecipientManifestationRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            ManifestationRoute,
            request,
            NFeJsonSerializerContext.Default.NFeRecipientManifestationRequest,
            NFeJsonSerializerContext.Default.NFeEventResponse,
            "recipient-manifestation",
            cancellationToken);
    }

    public Task<NFeEventResponse> RegisterIbsCbsEventAsync(
        NFeIbsCbsEventRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            IbsCbsEventRoute,
            request,
            NFeJsonSerializerContext.Default.NFeIbsCbsEventRequest,
            NFeJsonSerializerContext.Default.NFeEventResponse,
            "ibs-cbs-event",
            cancellationToken);
    }

    public Task<NFeEventResponse> CancelIbsCbsEventAsync(
        NFeIbsCbsEventCancellationRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Put,
            IbsCbsEventCancellationRoute,
            request,
            NFeJsonSerializerContext.Default.NFeIbsCbsEventCancellationRequest,
            NFeJsonSerializerContext.Default.NFeEventResponse,
            "cancel-ibs-cbs-event",
            cancellationToken);
    }

    public Task<NFeEventResponse> RegisterFinancialReconciliationAsync(
        NFeFinancialReconciliationRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            ReconciliationRoute,
            request,
            NFeJsonSerializerContext.Default
                .NFeFinancialReconciliationRequest,
            NFeJsonSerializerContext.Default.NFeEventResponse,
            "financial-reconciliation",
            cancellationToken);
    }

    public Task<NFeEventResponse> CancelFinancialReconciliationAsync(
        NFeFinancialReconciliationCancellationRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Put,
            ReconciliationCancellationRoute,
            request,
            NFeJsonSerializerContext.Default
                .NFeFinancialReconciliationCancellationRequest,
            NFeJsonSerializerContext.Default.NFeEventResponse,
            "cancel-financial-reconciliation",
            cancellationToken);
    }

    public Task<NFeSefazStatusResponse> GetSefazStatusAsync(
        CancellationToken cancellationToken = default)
    {
        return SendWithoutBodyAsync(
            SefazRoute,
            NFeJsonSerializerContext.Default.NFeSefazStatusResponse,
            "sefaz-status",
            cancellationToken);
    }

    public Task<NFeCertificateStatusResponse> GetCertificateStatusAsync(
        CancellationToken cancellationToken = default)
    {
        return SendWithoutBodyAsync(
            CertificateRoute,
            NFeJsonSerializerContext.Default.NFeCertificateStatusResponse,
            "certificate-status",
            cancellationToken);
    }

    public Task<NFeDocumentResponse> QueryByAccessKeyAsync(
        string accessKey,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.ValidateAccessKey(accessKey);
        return SendAsync(
            HttpMethod.Get,
            QueryRoute,
            new NFeAccessKeyQuery(accessKey),
            NFeJsonSerializerContext.Default.NFeAccessKeyQuery,
            NFeJsonSerializerContext.Default.NFeDocumentResponse,
            "query-by-access-key",
            cancellationToken);
    }

    public Task<NFeDocumentResponse> QueryByUuidAsync(
        Guid uuid,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.ValidateUuid(uuid);
        return SendAsync(
            HttpMethod.Get,
            QueryRoute,
            new NFeUuidQuery(uuid),
            NFeJsonSerializerContext.Default.NFeUuidQuery,
            NFeJsonSerializerContext.Default.NFeDocumentResponse,
            "query-by-uuid",
            cancellationToken);
    }

    public async Task<IReadOnlyList<NFeDocumentResponse>> QueryByOrderIdAsync(
        string orderId,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.ValidateOrderId(orderId);
        var result = await SendAsync(
                HttpMethod.Get,
                QueryRoute,
                new NFeOrderIdQuery(orderId),
                NFeJsonSerializerContext.Default.NFeOrderIdQuery,
                NFeJsonSerializerContext.Default.NFeDocumentResponseArray,
                "query-by-order-id",
                cancellationToken)
            .ConfigureAwait(false);
        return result;
    }

    public Task<NFeCancellationResponse> CancelAsync(
        NFeCancellationRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Put,
            CancellationRoute,
            request,
            NFeJsonSerializerContext.Default.NFeCancellationRequest,
            NFeJsonSerializerContext.Default.NFeCancellationResponse,
            "cancel",
            cancellationToken);
    }

    public Task<NFeNumberInvalidationResponse> InvalidateNumbersAsync(
        NFeNumberInvalidationRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Put,
            InvalidationRoute,
            request,
            NFeJsonSerializerContext.Default.NFeNumberInvalidationRequest,
            NFeJsonSerializerContext.Default.NFeNumberInvalidationResponse,
            "invalidate-numbers",
            cancellationToken);
    }

    public Task<NFeTaxClassResponse> UpsertTaxClassAsync(
        NFeTaxClassUpsertRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            TaxClassRoute,
            request,
            NFeJsonSerializerContext.Default.NFeTaxClassUpsertRequest,
            NFeJsonSerializerContext.Default.NFeTaxClassResponse,
            "upsert-tax-class",
            cancellationToken);
    }

    public async Task<IReadOnlyList<NFeTaxClassResponse>> ListTaxClassesAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await SendWithoutBodyAsync(
                TaxClassRoute,
                NFeJsonSerializerContext.Default.NFeTaxClassResponseArray,
                "list-tax-classes",
                cancellationToken)
            .ConfigureAwait(false);
        return result;
    }

    public async Task<IReadOnlyList<NFeTaxClassDeleteResponse>>
        DeleteTaxClassesAsync(
            NFeTaxClassDeleteRequest request,
            CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        var result = await SendAsync(
                HttpMethod.Delete,
                TaxClassRoute,
                request,
                NFeJsonSerializerContext.Default.NFeTaxClassDeleteRequest,
                NFeJsonSerializerContext.Default.NFeTaxClassDeleteResponseArray,
                "delete-tax-classes",
                cancellationToken)
            .ConfigureAwait(false);
        return result;
    }

    public Task<NFeReportResponse> ExportReportAsync(
        NFeReportRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            ReportRoute,
            request,
            NFeJsonSerializerContext.Default.NFeReportRequest,
            NFeJsonSerializerContext.Default.NFeReportResponse,
            "export-report",
            cancellationToken);
    }

    public Task<NFeCompanyUpdateResponse> UpdateCompanyAsync(
        NFeCompanyUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            CompanyRoute,
            request,
            NFeJsonSerializerContext.Default.NFeCompanyUpdateRequest,
            NFeJsonSerializerContext.Default.NFeCompanyUpdateResponse,
            "update-company",
            cancellationToken);
    }

    public Task<FiscalDocumentFile> DownloadAsync(
        Uri documentUrl,
        CancellationToken cancellationToken = default)
    {
        NFeRequestValidator.ValidateDownloadUri(documentUrl);
        var request = new HttpRequestMessage(HttpMethod.Get, documentUrl);
        return transport.DownloadAsync(
            WebManiaTransportKind.ApiV1,
            request,
            Product,
            "download",
            cancellationToken);
    }

    private Task<TResponse> SendWithoutBodyAsync<TResponse>(
        string route,
        JsonTypeInfo<TResponse> responseTypeInfo,
        string operation,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, route);
        return transport.SendAsync(
            WebManiaTransportKind.ApiV1,
            request,
            responseTypeInfo,
            Product,
            operation,
            cancellationToken);
    }

    private Task<TResponse> SendAsync<TRequest, TResponse>(
        HttpMethod method,
        string route,
        TRequest value,
        JsonTypeInfo<TRequest> requestTypeInfo,
        JsonTypeInfo<TResponse> responseTypeInfo,
        string operation,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(method, route)
        {
            Content = JsonContent.Create(value, requestTypeInfo),
        };

        return transport.SendAsync(
            WebManiaTransportKind.ApiV1,
            request,
            responseTypeInfo,
            Product,
            operation,
            cancellationToken);
    }
}
