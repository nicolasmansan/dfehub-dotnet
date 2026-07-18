namespace DFeHub.Client.UnitTests.TestDoubles;

internal sealed class RecordingHandler(
    Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> response)
    : HttpMessageHandler
{
    internal Dictionary<string, string[]> Headers { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    internal string? AuthorizationScheme { get; private set; }

    internal string? AuthorizationParameter { get; private set; }

    internal bool WasDisposed { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Headers.Clear();
        foreach (var header in request.Headers)
        {
            Headers[header.Key] = [.. header.Value];
        }

        AuthorizationScheme = request.Headers.Authorization?.Scheme;
        AuthorizationParameter = request.Headers.Authorization?.Parameter;

        return response(request, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        WasDisposed = true;
        base.Dispose(disposing);
    }
}

