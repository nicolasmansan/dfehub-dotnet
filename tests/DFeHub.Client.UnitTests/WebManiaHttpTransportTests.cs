using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using DFeHub.Client.UnitTests.TestDoubles;
using DFeHub.Diagnostics;
using DFeHub.Exceptions;
using DFeHub.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace DFeHub.Client.UnitTests;

public sealed class WebManiaHttpTransportTests
{
    [Fact]
    public async Task Success_response_is_deserialized_with_supplied_type_info()
    {
        var handler = HandlerReturning(
            JsonResponse(
                HttpStatusCode.OK,
                """{"Id":"abc","Status":"approved"}"""));
        var (transport, factory) = CreateTransport(handler);

        var result = await transport.SendAsync(
            WebManiaTransportKind.ApiV2,
            new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
            TestSerializationContext.Default.TestResponse,
            "NFSe",
            "Query",
            CancellationToken.None);

        Assert.Equal(new TestResponse("abc", "approved"), result);
        Assert.Equal(WebManiaHttpClientNames.ApiV2, factory.RequestedName);
    }

    [Fact]
    public async Task Each_transport_kind_selects_its_named_http_client()
    {
        var cases = new[]
        {
            (WebManiaTransportKind.ApiV1, WebManiaHttpClientNames.ApiV1),
            (WebManiaTransportKind.ApiV2, WebManiaHttpClientNames.ApiV2),
            (WebManiaTransportKind.Validator, WebManiaHttpClientNames.Validator),
            (WebManiaTransportKind.B2B, WebManiaHttpClientNames.B2B),
        };

        foreach (var (kind, expectedName) in cases)
        {
            var handler = HandlerReturning(
                JsonResponse(
                    HttpStatusCode.OK,
                    """{"Id":"abc","Status":"approved"}"""));
            var (transport, factory) = CreateTransport(handler);

            await transport.SendAsync(
                kind,
                new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
                TestSerializationContext.Default.TestResponse,
                "Product",
                "Operation",
                CancellationToken.None);

            Assert.Equal(expectedName, factory.RequestedName);
        }
    }

    [Fact]
    public async Task Api_error_is_exposed_without_transport_wrapping()
    {
        var handler = HandlerReturning(
            JsonResponse(
                HttpStatusCode.BadRequest,
                """{"error":"Documento inválido."}"""));
        var (transport, _) = CreateTransport(handler);

        var exception = await Assert.ThrowsAsync<WebManiaApiException>(
            () => transport.SendAsync(
                WebManiaTransportKind.ApiV1,
                new HttpRequestMessage(HttpMethod.Post, "https://example.test"),
                TestSerializationContext.Default.TestResponse,
                "NFe",
                "Issue",
                CancellationToken.None));

        Assert.Equal("Documento inválido.", exception.Message);
    }

    [Theory]
    [InlineData("{invalid")]
    [InlineData("null")]
    public async Task Invalid_or_empty_json_has_transport_exception(string json)
    {
        var handler = HandlerReturning(
            JsonResponse(HttpStatusCode.OK, json));
        var (transport, _) = CreateTransport(handler);

        var exception = await Assert.ThrowsAsync<WebManiaTransportException>(
            () => transport.SendAsync(
                WebManiaTransportKind.ApiV2,
                new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
                TestSerializationContext.Default.TestResponse,
                "NFSe",
                "Query",
                CancellationToken.None));

        Assert.Contains("JSON", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Http_request_failure_has_transport_exception()
    {
        var handler = new RecordingHandler(
            (_, _) => Task.FromException<HttpResponseMessage>(
                new HttpRequestException("socket failed")));
        var (transport, _) = CreateTransport(handler);

        var exception = await Assert.ThrowsAsync<WebManiaTransportException>(
            () => transport.SendAsync(
                WebManiaTransportKind.ApiV2,
                new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
                TestSerializationContext.Default.TestResponse,
                "NFSe",
                "Query",
                CancellationToken.None));

        Assert.IsType<HttpRequestException>(exception.InnerException);
    }

    [Fact]
    public async Task Caller_cancellation_is_not_wrapped()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var handler = new RecordingHandler(
            (_, token) => Task.FromCanceled<HttpResponseMessage>(token));
        var (transport, _) = CreateTransport(handler);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => transport.SendAsync(
                WebManiaTransportKind.ApiV2,
                new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
                TestSerializationContext.Default.TestResponse,
                "NFSe",
                "Query",
                cancellation.Token));
    }

    [Fact]
    public async Task Timeout_is_wrapped_as_transport_exception()
    {
        var handler = new RecordingHandler(
            (_, _) => Task.FromException<HttpResponseMessage>(
                new OperationCanceledException("timeout")));
        var (transport, _) = CreateTransport(handler);

        var exception = await Assert.ThrowsAsync<WebManiaTransportException>(
            () => transport.SendAsync(
                WebManiaTransportKind.ApiV2,
                new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
                TestSerializationContext.Default.TestResponse,
                "NFSe",
                "Query",
                CancellationToken.None));

        Assert.Contains("timed out", exception.Message);
    }

    [Fact]
    public async Task Download_streams_metadata_and_owns_response_lifetime()
    {
        var stream = new TrackingStream(Encoding.UTF8.GetBytes("<xml />"));
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StreamContent(stream),
        };
        response.Content.Headers.ContentType =
            new MediaTypeHeaderValue("application/xml");
        response.Content.Headers.ContentDisposition =
            new ContentDispositionHeaderValue("attachment")
            {
                FileNameStar = "nota.xml",
            };
        var handler = HandlerReturning(response);
        var (transport, factory) = CreateTransport(handler);

        var file = await transport.DownloadAsync(
            WebManiaTransportKind.ApiV1,
            new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
            "NFe",
            "Download",
            CancellationToken.None);

        Assert.Equal("nota.xml", file.FileName);
        Assert.Equal("application/xml", file.MediaType);
        Assert.NotNull(file.Length);
        Assert.False(stream.WasDisposed);
        Assert.Equal(WebManiaHttpClientNames.ApiV1, factory.RequestedName);

        await file.DisposeAsync();
        await file.DisposeAsync();

        Assert.True(stream.WasDisposedAsync);
    }

    [Fact]
    public async Task Synchronous_download_disposal_is_idempotent()
    {
        var stream = new TrackingStream([1, 2, 3]);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StreamContent(stream),
        };
        var (transport, _) = CreateTransport(HandlerReturning(response));
        var file = await transport.DownloadAsync(
            WebManiaTransportKind.Validator,
            new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
            "Validator",
            "Download",
            CancellationToken.None);

        file.Dispose();
        file.Dispose();

        Assert.True(stream.WasDisposed);
    }

    [Fact]
    public async Task Download_api_error_is_mapped()
    {
        var (transport, _) = CreateTransport(
            HandlerReturning(
                JsonResponse(
                    HttpStatusCode.NotFound,
                    """{"error":"Arquivo não encontrado."}""")));

        var exception = await Assert.ThrowsAsync<WebManiaApiException>(
            () => transport.DownloadAsync(
                WebManiaTransportKind.ApiV1,
                new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
                "NFe",
                "Download",
                CancellationToken.None));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task Download_http_failure_and_timeout_are_wrapped()
    {
        var httpFailure = CreateTransport(
            new RecordingHandler(
                (_, _) => Task.FromException<HttpResponseMessage>(
                    new HttpRequestException("network")))).Transport;
        var timeout = CreateTransport(
            new RecordingHandler(
                (_, _) => Task.FromException<HttpResponseMessage>(
                    new OperationCanceledException("timeout")))).Transport;

        var httpException = await Assert.ThrowsAsync<WebManiaTransportException>(
            () => httpFailure.DownloadAsync(
                WebManiaTransportKind.ApiV1,
                new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
                "NFe",
                "Download",
                CancellationToken.None));
        var timeoutException =
            await Assert.ThrowsAsync<WebManiaTransportException>(
                () => timeout.DownloadAsync(
                    WebManiaTransportKind.ApiV1,
                    new HttpRequestMessage(
                        HttpMethod.Get,
                        "https://example.test"),
                    "NFe",
                    "Download",
                    CancellationToken.None));

        Assert.IsType<HttpRequestException>(httpException.InnerException);
        Assert.Contains("timed out", timeoutException.Message);
    }

    [Fact]
    public async Task Activity_contains_only_low_cardinality_operation_metadata()
    {
        Activity? stoppedActivity = null;
        using var listener = new ActivityListener
        {
            ShouldListenTo = source =>
                source.Name == WebManiaTelemetry.ActivitySourceName,
            Sample = static (ref ActivityCreationOptions<ActivityContext> _) =>
                ActivitySamplingResult.AllData,
            ActivityStopped = activity => stoppedActivity = activity,
        };
        ActivitySource.AddActivityListener(listener);
        var (transport, _) = CreateTransport(
            HandlerReturning(
                JsonResponse(
                    HttpStatusCode.OK,
                    """{"Id":"abc","Status":"approved"}""")));

        await transport.SendAsync(
            WebManiaTransportKind.ApiV2,
            new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
            TestSerializationContext.Default.TestResponse,
            "NFSe",
            "Query",
            CancellationToken.None);

        Assert.NotNull(stoppedActivity);
        Assert.Equal(
            "NFSe",
            stoppedActivity.GetTagItem("webmania.product"));
        Assert.Equal(
            "Query",
            stoppedActivity.GetTagItem("webmania.operation"));
        Assert.Equal(
            200,
            stoppedActivity.GetTagItem("http.response.status_code"));
        Assert.DoesNotContain(
            stoppedActivity.TagObjects,
            tag => tag.Key.Contains("payload", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Invalid_arguments_and_unknown_transport_are_rejected()
    {
        var (transport, _) = CreateTransport(
            HandlerReturning(
                JsonResponse(
                    HttpStatusCode.OK,
                    """{"Id":"abc","Status":"approved"}""")));

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => transport.SendAsync(
                WebManiaTransportKind.ApiV2,
                null!,
                TestSerializationContext.Default.TestResponse,
                "NFSe",
                "Query",
                CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => transport.SendAsync<TestResponse>(
                WebManiaTransportKind.ApiV2,
                new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
                null!,
                "NFSe",
                "Query",
                CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(
            () => transport.SendAsync(
                WebManiaTransportKind.ApiV2,
                new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
                TestSerializationContext.Default.TestResponse,
                "",
                "Query",
                CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => transport.SendAsync(
                (WebManiaTransportKind)999,
                new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
                TestSerializationContext.Default.TestResponse,
                "NFSe",
                "Query",
                CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => transport.DownloadAsync(
                WebManiaTransportKind.ApiV1,
                null!,
                "NFe",
                "Download",
                CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(
            () => transport.DownloadAsync(
                WebManiaTransportKind.ApiV1,
                new HttpRequestMessage(HttpMethod.Get, "https://example.test"),
                "NFe",
                " ",
                CancellationToken.None));
    }

    private static RecordingHandler HandlerReturning(
        HttpResponseMessage response)
    {
        return new RecordingHandler(
            (_, _) => Task.FromResult(response));
    }

    private static HttpResponseMessage JsonResponse(
        HttpStatusCode statusCode,
        string json)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };
    }

    private static (
        WebManiaHttpTransport Transport,
        StubHttpClientFactory Factory) CreateTransport(
        HttpMessageHandler handler)
    {
        var client = new HttpClient(handler, disposeHandler: true);
        var factory = new StubHttpClientFactory(client);
        var transport = new WebManiaHttpTransport(
            factory,
            NullLogger<WebManiaHttpTransport>.Instance);

        return (transport, factory);
    }
}
