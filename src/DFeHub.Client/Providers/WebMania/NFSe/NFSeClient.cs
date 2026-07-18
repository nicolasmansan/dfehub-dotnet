using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;
using DFeHub.Http;
using DFeHub.Providers.WebMania;
using Microsoft.Extensions.Options;

namespace DFeHub.Providers.WebMania.NFSe;

internal sealed class NFSeClient(
    IOptionsMonitor<WebManiaOptions> options,
    IWebManiaHttpTransport transport)
    : WebManiaProductClient(options, WebManiaCredentialGroup.ApiV2),
        INFSeClient
{
    private const string Product = "nfse";
    private const string StatusRoute = "2/nfse/status";
    private const string IssueRoute = "2/nfse/emissao";
    private const string QueryRoute = "2/nfse/consulta";
    private const string CancellationRoute = "2/nfse/cancelar";
    private const string SubstitutionRoute = "2/nfse/substituir";
    private const string ManifestationRoute = "2/nfse/manifestar";

    public Task<NFSeProviderStatusResponse> GetMunicipalityStatusAsync(
        CancellationToken cancellationToken = default)
    {
        return SendWithoutBodyAsync(
            StatusRoute,
            NFSeJsonSerializerContext.Default.NFSeProviderStatusResponse,
            "municipality-status",
            cancellationToken);
    }

    public Task<NFSeDocumentResponse> IssueAsync(
        NFSeIssueRequest request,
        CancellationToken cancellationToken = default)
    {
        NFSeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            IssueRoute,
            request,
            NFSeJsonSerializerContext.Default.NFSeIssueRequest,
            NFSeJsonSerializerContext.Default.NFSeDocumentResponse,
            "issue",
            cancellationToken);
    }

    public Task<NFSeDocumentResponse> QueryByUuidAsync(
        Guid uuid,
        CancellationToken cancellationToken = default)
    {
        NFSeRequestValidator.ValidateUuid(uuid);
        return SendWithoutBodyAsync(
            $"{QueryRoute}/{uuid:D}",
            NFSeJsonSerializerContext.Default.NFSeDocumentResponse,
            "query-by-uuid",
            cancellationToken);
    }

    public Task<NFSeDocumentResponse> CancelAsync(
        NFSeCancellationRequest request,
        CancellationToken cancellationToken = default)
    {
        NFSeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Put,
            CancellationRoute,
            request,
            NFSeJsonSerializerContext.Default.NFSeCancellationRequest,
            NFSeJsonSerializerContext.Default.NFSeDocumentResponse,
            "cancel",
            cancellationToken);
    }

    public Task<NFSeDocumentResponse> SubstituteAsync(
        NFSeSubstitutionRequest request,
        CancellationToken cancellationToken = default)
    {
        NFSeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            SubstitutionRoute,
            request,
            NFSeJsonSerializerContext.Default.NFSeSubstitutionRequest,
            NFSeJsonSerializerContext.Default.NFSeDocumentResponse,
            "substitute",
            cancellationToken);
    }

    public Task<NFSeDocumentResponse> ManifestAsync(
        NFSeManifestationRequest request,
        CancellationToken cancellationToken = default)
    {
        NFSeRequestValidator.Validate(request);
        return SendAsync(
            HttpMethod.Post,
            ManifestationRoute,
            request,
            NFSeJsonSerializerContext.Default.NFSeManifestationRequest,
            NFSeJsonSerializerContext.Default.NFSeDocumentResponse,
            "manifest",
            cancellationToken);
    }

    public Task<FiscalDocumentFile> DownloadAsync(
        Uri documentUrl,
        CancellationToken cancellationToken = default)
    {
        NFSeRequestValidator.ValidateDownloadUri(documentUrl);
        var request = new HttpRequestMessage(HttpMethod.Get, documentUrl);
        return transport.DownloadAsync(
            WebManiaTransportKind.ApiV2,
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
            WebManiaTransportKind.ApiV2,
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
            WebManiaTransportKind.ApiV2,
            request,
            responseTypeInfo,
            Product,
            operation,
            cancellationToken);
    }
}
