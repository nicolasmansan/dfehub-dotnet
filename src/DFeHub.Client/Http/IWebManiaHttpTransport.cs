using System.Text.Json.Serialization.Metadata;

namespace DFeHub.Http;

internal interface IWebManiaHttpTransport
{
    Task<TResponse> SendAsync<TResponse>(
        WebManiaTransportKind transportKind,
        HttpRequestMessage request,
        JsonTypeInfo<TResponse> responseTypeInfo,
        string product,
        string operation,
        CancellationToken cancellationToken);

    Task<FiscalDocumentFile> DownloadAsync(
        WebManiaTransportKind transportKind,
        HttpRequestMessage request,
        string product,
        string operation,
        CancellationToken cancellationToken);
}

