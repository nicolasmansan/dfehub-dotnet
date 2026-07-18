using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace DFeHub.Providers.WebMania;

internal static class WebManiaConfigurationBinder
{
    public static void Bind(
        IConfiguration configuration,
        WebManiaOptions options)
    {
        BindApiV1(configuration.GetSection("ApiV1"), options.ApiV1);
        BindApiV2(configuration.GetSection("ApiV2"), options.ApiV2);
        BindValidator(
            configuration.GetSection("Validator"),
            options.Validator);
        BindApiV1(configuration.GetSection("B2B"), options.B2B);
        BindEndpoints(
            configuration.GetSection("Endpoints"),
            options.Endpoints);

        var timeout = configuration["Timeout"];
        if (timeout is not null)
        {
            options.Timeout = TimeSpan.Parse(
                timeout,
                CultureInfo.InvariantCulture);
        }
    }

    private static void BindApiV1(
        IConfiguration configuration,
        WebManiaApiV1Credentials credentials)
    {
        credentials.ConsumerKey =
            configuration["ConsumerKey"] ?? credentials.ConsumerKey;
        credentials.ConsumerSecret =
            configuration["ConsumerSecret"] ?? credentials.ConsumerSecret;
        credentials.AccessToken =
            configuration["AccessToken"] ?? credentials.AccessToken;
        credentials.AccessTokenSecret =
            configuration["AccessTokenSecret"] ??
            credentials.AccessTokenSecret;
    }

    private static void BindApiV2(
        IConfiguration configuration,
        WebManiaApiV2Credentials credentials)
    {
        credentials.AccessToken =
            configuration["AccessToken"] ?? credentials.AccessToken;
    }

    private static void BindValidator(
        IConfiguration configuration,
        WebManiaValidatorCredentials credentials)
    {
        credentials.Token = configuration["Token"] ?? credentials.Token;
    }

    private static void BindEndpoints(
        IConfiguration configuration,
        WebManiaEndpointOptions endpoints)
    {
        endpoints.ApiV1 = BindUri(configuration["ApiV1"], endpoints.ApiV1);
        endpoints.ApiV2 = BindUri(configuration["ApiV2"], endpoints.ApiV2);
        endpoints.Validator = BindUri(
            configuration["Validator"],
            endpoints.Validator);
    }

    private static Uri BindUri(string? value, Uri currentValue)
    {
        return value is null
            ? currentValue
            : new Uri(value, UriKind.Absolute);
    }
}
