using DFeHub.Providers.WebMania;
using Microsoft.Extensions.Options;

namespace DFeHub.Http.Authentication;

internal sealed class ValidatorAuthenticationHandler(
    IOptionsMonitor<WebManiaOptions> options)
    : DelegatingHandler
{
    internal const string TokenHeader = "X-Token";

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var currentOptions = options.CurrentValue;
        WebManiaCredentialValidator.EnsureConfigured(
            currentOptions,
            WebManiaCredentialGroup.Validator);

        request.Headers.Remove(TokenHeader);
        request.Headers.TryAddWithoutValidation(
            TokenHeader,
            currentOptions.Validator.Token);

        return base.SendAsync(request, cancellationToken);
    }
}

