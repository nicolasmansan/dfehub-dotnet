using System.Net.Http.Headers;

namespace DFeHub;

/// <summary>
/// Represents a streamed fiscal document returned by Webmania.
/// </summary>
public sealed class FiscalDocumentFile : IDisposable, IAsyncDisposable
{
    private readonly HttpResponseMessage response;
    private int disposed;

    internal FiscalDocumentFile(
        HttpResponseMessage response,
        Stream content)
    {
        this.response = response;
        Content = content;
        FileName = GetFileName(response.Content.Headers.ContentDisposition);
        MediaType = response.Content.Headers.ContentType?.MediaType;
        Length = response.Content.Headers.ContentLength;
    }

    /// <summary>
    /// Gets the document content stream.
    /// </summary>
    public Stream Content { get; }

    /// <summary>
    /// Gets the provider-supplied file name, when available.
    /// </summary>
    public string? FileName { get; }

    /// <summary>
    /// Gets the response media type.
    /// </summary>
    public string? MediaType { get; }

    /// <summary>
    /// Gets the response content length, when available.
    /// </summary>
    public long? Length { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed, 1) != 0)
        {
            return;
        }

        Content.Dispose();
        response.Dispose();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref disposed, 1) != 0)
        {
            return;
        }

        await Content.DisposeAsync().ConfigureAwait(false);
        response.Dispose();
        GC.SuppressFinalize(this);
    }

    private static string? GetFileName(ContentDispositionHeaderValue? disposition)
    {
        var fileName = disposition?.FileNameStar ?? disposition?.FileName;
        return fileName?.Trim('"');
    }
}

