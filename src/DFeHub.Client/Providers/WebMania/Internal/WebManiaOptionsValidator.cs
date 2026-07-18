using Microsoft.Extensions.Options;

namespace DFeHub.Providers.WebMania;

internal sealed class WebManiaOptionsValidator : IValidateOptions<WebManiaOptions>
{
    private static readonly TimeSpan MaximumTimeout = TimeSpan.FromMinutes(10);

    public ValidateOptionsResult Validate(string? name, WebManiaOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var failures = new List<string>();

        ValidateEndpoint(options.Endpoints.ApiV1, "Endpoints:ApiV1", failures);
        ValidateEndpoint(options.Endpoints.ApiV2, "Endpoints:ApiV2", failures);
        ValidateEndpoint(
            options.Endpoints.Validator,
            "Endpoints:Validator",
            failures);

        if (options.Timeout <= TimeSpan.Zero || options.Timeout > MaximumTimeout)
        {
            failures.Add(
                $"Timeout must be greater than zero and no longer than " +
                $"{MaximumTimeout.TotalMinutes:0} minutes.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static void ValidateEndpoint(
        Uri? endpoint,
        string property,
        List<string> failures)
    {
        if (endpoint is null || !endpoint.IsAbsoluteUri)
        {
            failures.Add($"{property} must be an absolute URI.");
            return;
        }

        var isHttps = endpoint.Scheme == Uri.UriSchemeHttps;
        var isLoopbackHttp =
            endpoint.Scheme == Uri.UriSchemeHttp &&
            endpoint.IsLoopback;

        if (!isHttps && !isLoopbackHttp)
        {
            failures.Add(
                $"{property} must use HTTPS. HTTP is accepted only for loopback.");
        }
    }
}

