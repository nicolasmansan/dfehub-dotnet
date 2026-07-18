using DFeHub.Providers.WebMania;
using Microsoft.Extensions.Options;

namespace DFeHub.Http.Authentication;

internal sealed class ApiV1AuthenticationHandler : DelegatingHandler
{
    internal const string ConsumerKeyHeader = "X-Consumer-Key";
    internal const string ConsumerSecretHeader = "X-Consumer-Secret";
    internal const string AccessTokenHeader = "X-Access-Token";
    internal const string AccessTokenSecretHeader = "X-Access-Token-Secret";

    private readonly IOptionsMonitor<WebManiaOptions> options;
    private readonly WebManiaCredentialGroup credentialGroup;

    internal ApiV1AuthenticationHandler(
        IOptionsMonitor<WebManiaOptions> options,
        WebManiaCredentialGroup credentialGroup)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (credentialGroup is not (
            WebManiaCredentialGroup.ApiV1 or
            WebManiaCredentialGroup.B2B))
        {
            throw new ArgumentOutOfRangeException(
                nameof(credentialGroup),
                credentialGroup,
                "API 1.0 authentication accepts only ApiV1 or B2B credentials.");
        }

        this.options = options;
        this.credentialGroup = credentialGroup;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var currentOptions = options.CurrentValue;
        WebManiaCredentialValidator.EnsureConfigured(
            currentOptions,
            credentialGroup);

        var credentials = credentialGroup == WebManiaCredentialGroup.B2B
            ? currentOptions.B2B
            : currentOptions.ApiV1;

        SetHeader(request, ConsumerKeyHeader, credentials.ConsumerKey!);
        SetHeader(request, ConsumerSecretHeader, credentials.ConsumerSecret!);
        SetHeader(request, AccessTokenHeader, credentials.AccessToken!);
        SetHeader(
            request,
            AccessTokenSecretHeader,
            credentials.AccessTokenSecret!);

        return base.SendAsync(request, cancellationToken);
    }

    private static void SetHeader(
        HttpRequestMessage request,
        string name,
        string value)
    {
        request.Headers.Remove(name);
        request.Headers.TryAddWithoutValidation(name, value);
    }
}

