using System.Net.Http.Headers;
using DFeHub.Providers.WebMania;
using Microsoft.Extensions.Options;

namespace DFeHub.Http.Authentication;

internal sealed class ApiV2AuthenticationHandler(
    IOptionsMonitor<WebManiaOptions> options)
    : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var currentOptions = options.CurrentValue;
        WebManiaCredentialValidator.EnsureConfigured(
            currentOptions,
            WebManiaCredentialGroup.ApiV2);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            currentOptions.ApiV2.AccessToken);

        return base.SendAsync(request, cancellationToken);
    }
}

