using System.Net;
using DFeHub.Client.UnitTests.TestDoubles;
using DFeHub.Exceptions;
using DFeHub.Http.Authentication;
using DFeHub.Providers.WebMania;

namespace DFeHub.Client.UnitTests;

public sealed class AuthenticationHandlerTests
{
    [Fact]
    public async Task Api_v1_adds_exact_official_headers()
    {
        var options = WebManiaOptionsTests.CreateFullyConfiguredOptions();
        var recorder = CreateRecorder();
        var handler = new ApiV1AuthenticationHandler(
            new StaticOptionsMonitor<WebManiaOptions>(options),
            WebManiaCredentialGroup.ApiV1)
        {
            InnerHandler = recorder,
        };
        using var client = new HttpClient(handler);

        using var response = await client.GetAsync("https://example.test");

        Assert.Equal("consumer-key", Header(recorder, "X-Consumer-Key"));
        Assert.Equal(
            "consumer-secret",
            Header(recorder, "X-Consumer-Secret"));
        Assert.Equal("access-token", Header(recorder, "X-Access-Token"));
        Assert.Equal(
            "access-secret",
            Header(recorder, "X-Access-Token-Secret"));
    }

    [Fact]
    public async Task B2b_uses_its_own_credentials_and_replaces_existing_headers()
    {
        var options = WebManiaOptionsTests.CreateFullyConfiguredOptions();
        var recorder = CreateRecorder();
        var handler = new ApiV1AuthenticationHandler(
            new StaticOptionsMonitor<WebManiaOptions>(options),
            WebManiaCredentialGroup.B2B)
        {
            InnerHandler = recorder,
        };
        using var client = new HttpClient(handler);
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://example.test");
        request.Headers.TryAddWithoutValidation("X-Consumer-Key", "old-value");

        using var response = await client.SendAsync(request);

        Assert.Equal(
            "b2b-consumer-key",
            Header(recorder, "X-Consumer-Key"));
        Assert.Equal(
            "b2b-consumer-secret",
            Header(recorder, "X-Consumer-Secret"));
        Assert.Equal(
            "b2b-access-token",
            Header(recorder, "X-Access-Token"));
        Assert.Equal(
            "b2b-access-secret",
            Header(recorder, "X-Access-Token-Secret"));
        Assert.DoesNotContain("consumer-key", recorder.Headers.Values.SelectMany(x => x));
    }

    [Fact]
    public void Api_v1_rejects_incompatible_credential_group()
    {
        var options = new StaticOptionsMonitor<WebManiaOptions>(
            new WebManiaOptions());

        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ApiV1AuthenticationHandler(
                options,
                WebManiaCredentialGroup.ApiV2));
    }

    [Fact]
    public async Task Api_v1_rejects_missing_credentials_before_network()
    {
        var recorder = CreateRecorder();
        var handler = new ApiV1AuthenticationHandler(
            new StaticOptionsMonitor<WebManiaOptions>(new WebManiaOptions()),
            WebManiaCredentialGroup.ApiV1)
        {
            InnerHandler = recorder,
        };
        using var client = new HttpClient(handler);

        var exception = await Assert.ThrowsAsync<WebManiaConfigurationException>(
            () => client.GetAsync("https://example.test"));

        Assert.Contains("ApiV1", exception.Message);
        Assert.Empty(recorder.Headers);
    }

    [Fact]
    public async Task Api_v2_adds_bearer_authorization()
    {
        var options = WebManiaOptionsTests.CreateFullyConfiguredOptions();
        var recorder = CreateRecorder();
        var handler = new ApiV2AuthenticationHandler(
            new StaticOptionsMonitor<WebManiaOptions>(options))
        {
            InnerHandler = recorder,
        };
        using var client = new HttpClient(handler);

        using var response = await client.GetAsync("https://example.test");

        Assert.Equal("Bearer", recorder.AuthorizationScheme);
        Assert.Equal("bearer-secret", recorder.AuthorizationParameter);
        Assert.DoesNotContain("X-Token", recorder.Headers.Keys);
    }

    [Fact]
    public async Task Api_v2_rejects_missing_token_before_network()
    {
        var recorder = CreateRecorder();
        var handler = new ApiV2AuthenticationHandler(
            new StaticOptionsMonitor<WebManiaOptions>(new WebManiaOptions()))
        {
            InnerHandler = recorder,
        };
        using var client = new HttpClient(handler);

        var exception = await Assert.ThrowsAsync<WebManiaConfigurationException>(
            () => client.GetAsync("https://example.test"));

        Assert.Contains("ApiV2", exception.Message);
    }

    [Fact]
    public async Task Validator_adds_x_token_and_replaces_existing_value()
    {
        var options = WebManiaOptionsTests.CreateFullyConfiguredOptions();
        var recorder = CreateRecorder();
        var handler = new ValidatorAuthenticationHandler(
            new StaticOptionsMonitor<WebManiaOptions>(options))
        {
            InnerHandler = recorder,
        };
        using var client = new HttpClient(handler);
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://example.test");
        request.Headers.TryAddWithoutValidation("X-Token", "old-value");

        using var response = await client.SendAsync(request);

        Assert.Equal("validator-secret", Header(recorder, "X-Token"));
        Assert.Null(recorder.AuthorizationScheme);
    }

    [Fact]
    public async Task Validator_rejects_missing_token_before_network()
    {
        var recorder = CreateRecorder();
        var handler = new ValidatorAuthenticationHandler(
            new StaticOptionsMonitor<WebManiaOptions>(new WebManiaOptions()))
        {
            InnerHandler = recorder,
        };
        using var client = new HttpClient(handler);

        var exception = await Assert.ThrowsAsync<WebManiaConfigurationException>(
            () => client.GetAsync("https://example.test"));

        Assert.Contains("Validator", exception.Message);
    }

    [Fact]
    public void Disposing_http_client_disposes_handler_pipeline()
    {
        var options = WebManiaOptionsTests.CreateFullyConfiguredOptions();
        var recorder = CreateRecorder();
        var handler = new ApiV2AuthenticationHandler(
            new StaticOptionsMonitor<WebManiaOptions>(options))
        {
            InnerHandler = recorder,
        };
        var client = new HttpClient(handler);

        client.Dispose();

        Assert.True(recorder.WasDisposed);
    }

    private static RecordingHandler CreateRecorder()
    {
        return new RecordingHandler(
            (_, _) => Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.OK)));
    }

    private static string Header(RecordingHandler recorder, string name)
    {
        return Assert.Single(recorder.Headers[name]);
    }
}

