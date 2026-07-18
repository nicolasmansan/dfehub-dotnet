using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using DFeHub.Diagnostics;
using DFeHub.Exceptions;
using Microsoft.Extensions.Logging;

namespace DFeHub.Http;

internal sealed class WebManiaHttpTransport(
    IHttpClientFactory httpClientFactory,
    ILogger<WebManiaHttpTransport> logger)
    : IWebManiaHttpTransport
{
    public async Task<TResponse> SendAsync<TResponse>(
        WebManiaTransportKind transportKind,
        HttpRequestMessage request,
        JsonTypeInfo<TResponse> responseTypeInfo,
        string product,
        string operation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(responseTypeInfo);
        ArgumentException.ThrowIfNullOrWhiteSpace(product);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);

        using (request)
        using (var activity = StartActivity(product, operation))
        {
            var stopwatch = Stopwatch.StartNew();
            HttpResponseMessage? response = null;

            try
            {
                response = await SendCoreAsync(
                        transportKind,
                        request,
                        cancellationToken)
                    .ConfigureAwait(false);

                SetResponseTags(activity, response.StatusCode);
                RecordRequest(
                    product,
                    operation,
                    (int)response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds);

                WebManiaLog.RequestCompleted(
                    logger,
                    product,
                    operation,
                    (int)response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds);

                if (!response.IsSuccessStatusCode)
                {
                    throw await WebManiaApiExceptionFactory.CreateAsync(
                            response,
                            product,
                            operation,
                            cancellationToken)
                        .ConfigureAwait(false);
                }

                try
                {
                    var result = await response.Content
                        .ReadFromJsonAsync(responseTypeInfo, cancellationToken)
                        .ConfigureAwait(false);

                    return result ??
                        throw new WebManiaTransportException(
                            $"Webmania {product}/{operation} returned an empty " +
                            "JSON response.");
                }
                catch (JsonException exception)
                {
                    WebManiaLog.ResponseDecodingFailed(
                        logger,
                        exception,
                        product,
                        operation);

                    throw new WebManiaTransportException(
                        $"Webmania {product}/{operation} returned invalid JSON.",
                        exception);
                }
                catch (NotSupportedException exception)
                {
                    WebManiaLog.ResponseDecodingFailed(
                        logger,
                        exception,
                        product,
                        operation);

                    throw new WebManiaTransportException(
                        $"Webmania {product}/{operation} returned an unsupported " +
                        "JSON response.",
                        exception);
                }
            }
            catch (HttpRequestException exception)
            {
                activity?.SetStatus(ActivityStatusCode.Error);
                RecordRequest(
                    product,
                    operation,
                    0,
                    stopwatch.Elapsed.TotalMilliseconds);
                WebManiaLog.TransportFailed(
                    logger,
                    exception,
                    product,
                    operation);

                throw new WebManiaTransportException(
                    $"The HTTP request for Webmania {product}/{operation} failed.",
                    exception);
            }
            catch (OperationCanceledException exception)
                when (!cancellationToken.IsCancellationRequested)
            {
                activity?.SetStatus(ActivityStatusCode.Error);
                RecordRequest(
                    product,
                    operation,
                    0,
                    stopwatch.Elapsed.TotalMilliseconds);
                WebManiaLog.TransportFailed(
                    logger,
                    exception,
                    product,
                    operation);

                throw new WebManiaTransportException(
                    $"The HTTP request for Webmania {product}/{operation} timed out.",
                    exception);
            }
            finally
            {
                response?.Dispose();
            }
        }
    }

    public async Task<FiscalDocumentFile> DownloadAsync(
        WebManiaTransportKind transportKind,
        HttpRequestMessage request,
        string product,
        string operation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(product);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);

        using (request)
        using (var activity = StartActivity(product, operation))
        {
            var stopwatch = Stopwatch.StartNew();
            HttpResponseMessage? response = null;

            try
            {
                response = await SendCoreAsync(
                        transportKind,
                        request,
                        cancellationToken)
                    .ConfigureAwait(false);

                SetResponseTags(activity, response.StatusCode);
                RecordRequest(
                    product,
                    operation,
                    (int)response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds);

                WebManiaLog.RequestCompleted(
                    logger,
                    product,
                    operation,
                    (int)response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds);

                if (!response.IsSuccessStatusCode)
                {
                    throw await WebManiaApiExceptionFactory.CreateAsync(
                            response,
                            product,
                            operation,
                            cancellationToken)
                        .ConfigureAwait(false);
                }

                var stream = await response.Content
                    .ReadAsStreamAsync(cancellationToken)
                    .ConfigureAwait(false);

                var result = new FiscalDocumentFile(response, stream);
                response = null;
                return result;
            }
            catch (HttpRequestException exception)
            {
                activity?.SetStatus(ActivityStatusCode.Error);
                RecordRequest(
                    product,
                    operation,
                    0,
                    stopwatch.Elapsed.TotalMilliseconds);
                WebManiaLog.TransportFailed(
                    logger,
                    exception,
                    product,
                    operation);

                throw new WebManiaTransportException(
                    $"The HTTP download for Webmania {product}/{operation} failed.",
                    exception);
            }
            catch (OperationCanceledException exception)
                when (!cancellationToken.IsCancellationRequested)
            {
                activity?.SetStatus(ActivityStatusCode.Error);
                RecordRequest(
                    product,
                    operation,
                    0,
                    stopwatch.Elapsed.TotalMilliseconds);
                WebManiaLog.TransportFailed(
                    logger,
                    exception,
                    product,
                    operation);

                throw new WebManiaTransportException(
                    $"The HTTP download for Webmania {product}/{operation} timed out.",
                    exception);
            }
            finally
            {
                response?.Dispose();
            }
        }
    }

    private async Task<HttpResponseMessage> SendCoreAsync(
        WebManiaTransportKind transportKind,
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(GetClientName(transportKind));
        return await client
            .SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken)
            .ConfigureAwait(false);
    }

    private static string GetClientName(WebManiaTransportKind transportKind)
    {
        return transportKind switch
        {
            WebManiaTransportKind.ApiV1 => WebManiaHttpClientNames.ApiV1,
            WebManiaTransportKind.ApiV2 => WebManiaHttpClientNames.ApiV2,
            WebManiaTransportKind.Validator => WebManiaHttpClientNames.Validator,
            WebManiaTransportKind.B2B => WebManiaHttpClientNames.B2B,
            _ => throw new ArgumentOutOfRangeException(
                nameof(transportKind),
                transportKind,
                "Unknown Webmania HTTP transport."),
        };
    }

    private static Activity? StartActivity(string product, string operation)
    {
        var activity = WebManiaDiagnostics.ActivitySource.StartActivity(
            $"webmania.{product}.{operation}",
            ActivityKind.Client);

        activity?.SetTag("webmania.product", product);
        activity?.SetTag("webmania.operation", operation);
        return activity;
    }

    private static void SetResponseTags(
        Activity? activity,
        System.Net.HttpStatusCode statusCode)
    {
        activity?.SetTag("http.response.status_code", (int)statusCode);
        activity?.SetStatus(
            (int)statusCode >= 400
                ? ActivityStatusCode.Error
                : ActivityStatusCode.Ok);
    }

    private static void RecordRequest(
        string product,
        string operation,
        int statusCode,
        double elapsedMilliseconds)
    {
        var tags = new TagList
        {
            { "webmania.product", product },
            { "webmania.operation", operation },
            { "http.response.status_code", statusCode },
        };

        WebManiaDiagnostics.RequestCounter.Add(1, tags);
        WebManiaDiagnostics.RequestDuration.Record(elapsedMilliseconds, tags);
    }
}
