using System.Text.Json.Serialization.Metadata;
using DFeHub.Http;

namespace DFeHub.Client.UnitTests.TestDoubles;

internal sealed class RecordingNFSeTransport : IWebManiaHttpTransport
{
    internal object? Response { get; set; }

    internal WebManiaTransportKind TransportKind { get; private set; }

    internal HttpMethod? Method { get; private set; }

    internal Uri? Uri { get; private set; }

    internal string? Body { get; private set; }

    internal string? Product { get; private set; }

    internal string? Operation { get; private set; }

    internal CancellationToken CancellationToken { get; private set; }

    public async Task<TResponse> SendAsync<TResponse>(
        WebManiaTransportKind transportKind,
        HttpRequestMessage request,
        JsonTypeInfo<TResponse> responseTypeInfo,
        string product,
        string operation,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Capture(transportKind, request, product, operation, cancellationToken);
        Body = request.Content is null
            ? null
            : await request.Content
                .ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

        return Response is TResponse response
            ? response
            : throw new InvalidOperationException(
                $"No {typeof(TResponse).Name} response was configured.");
    }

    public Task<FiscalDocumentFile> DownloadAsync(
        WebManiaTransportKind transportKind,
        HttpRequestMessage request,
        string product,
        string operation,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Capture(transportKind, request, product, operation, cancellationToken);

        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new ByteArrayContent([1, 2, 3]),
        };
        var file = new FiscalDocumentFile(
            response,
            new MemoryStream([1, 2, 3], writable: false));
        return Task.FromResult(file);
    }

    private void Capture(
        WebManiaTransportKind transportKind,
        HttpRequestMessage request,
        string product,
        string operation,
        CancellationToken cancellationToken)
    {
        TransportKind = transportKind;
        Method = request.Method;
        Uri = request.RequestUri;
        Product = product;
        Operation = operation;
        CancellationToken = cancellationToken;
    }
}
