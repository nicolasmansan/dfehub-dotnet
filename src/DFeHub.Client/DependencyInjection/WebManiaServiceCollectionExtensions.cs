using System.Net.Http.Headers;
using DFeHub.Http;
using DFeHub.Http.Authentication;
using DFeHub.Providers.WebMania;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Registers DFeHub's Webmania clients.
/// </summary>
public static class WebManiaServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Webmania clients and binds their options from configuration.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="configuration">The Webmania configuration section.</param>
    /// <returns>The supplied service collection.</returns>
    public static IServiceCollection AddWebMania(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<WebManiaOptions>()
            .Configure(
                options => WebManiaConfigurationBinder.Bind(
                    configuration,
                    options));
        services.AddSingleton<IOptionsChangeTokenSource<WebManiaOptions>>(
            new ConfigurationChangeTokenSource<WebManiaOptions>(
                configuration));

        return AddWebManiaCore(services);
    }

    /// <summary>
    /// Registers all Webmania clients and configures their options.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="configure">The options configuration delegate.</param>
    /// <returns>The supplied service collection.</returns>
    public static IServiceCollection AddWebMania(
        this IServiceCollection services,
        Action<WebManiaOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services
            .AddOptions<WebManiaOptions>()
            .Configure(configure);

        return AddWebManiaCore(services);
    }

    private static IServiceCollection AddWebManiaCore(IServiceCollection services)
    {
        if (services.Any(
            descriptor =>
                descriptor.ServiceType == typeof(WebManiaRegistrationMarker)))
        {
            return services;
        }

        services.AddSingleton<WebManiaRegistrationMarker>();
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<
                IValidateOptions<WebManiaOptions>,
                WebManiaOptionsValidator>());

        RegisterHttpClients(services);
        RegisterProductClients(services);
        services.TryAddSingleton<IWebManiaHttpTransport, WebManiaHttpTransport>();
        services.TryAddSingleton<IWebManiaClient, WebManiaClient>();

        return services;
    }

    private static void RegisterHttpClients(IServiceCollection services)
    {
        services
            .AddHttpClient(
                WebManiaHttpClientNames.ApiV1,
                (provider, client) => ConfigureClient(
                    provider,
                    client,
                    static options => options.Endpoints.ApiV1))
            .AddHttpMessageHandler(
                provider => new ApiV1AuthenticationHandler(
                    provider.GetRequiredService<
                        IOptionsMonitor<WebManiaOptions>>(),
                    WebManiaCredentialGroup.ApiV1));

        services
            .AddHttpClient(
                WebManiaHttpClientNames.B2B,
                (provider, client) => ConfigureClient(
                    provider,
                    client,
                    static options => options.Endpoints.ApiV1))
            .AddHttpMessageHandler(
                provider => new ApiV1AuthenticationHandler(
                    provider.GetRequiredService<
                        IOptionsMonitor<WebManiaOptions>>(),
                    WebManiaCredentialGroup.B2B));

        services
            .AddHttpClient(
                WebManiaHttpClientNames.ApiV2,
                (provider, client) => ConfigureClient(
                    provider,
                    client,
                    static options => options.Endpoints.ApiV2))
            .AddHttpMessageHandler<ApiV2AuthenticationHandler>();

        services
            .AddHttpClient(
                WebManiaHttpClientNames.Validator,
                (provider, client) => ConfigureClient(
                    provider,
                    client,
                    static options => options.Endpoints.Validator))
            .AddHttpMessageHandler<ValidatorAuthenticationHandler>();

        services.TryAddTransient<ApiV2AuthenticationHandler>();
        services.TryAddTransient<ValidatorAuthenticationHandler>();
    }

    private static void RegisterProductClients(IServiceCollection services)
    {
        services.TryAddSingleton<INFeClient, NFeClient>();
        services.TryAddSingleton<INFSeClient, NFSeClient>();
        services.TryAddSingleton<ICTeClient, CTeClient>();
        services.TryAddSingleton<IMDFeClient, MDFeClient>();
        services.TryAddSingleton<INFComClient, NFComClient>();
        services.TryAddSingleton<IDCeClient, DCeClient>();
        services.TryAddSingleton<IMonitorFiscalClient, MonitorFiscalClient>();
        services.TryAddSingleton<IValidatorFiscalClient, ValidatorFiscalClient>();
        services.TryAddSingleton<IB2BClient, B2BClient>();
    }

    private static void ConfigureClient(
        IServiceProvider provider,
        HttpClient client,
        Func<WebManiaOptions, Uri> endpointSelector)
    {
        var options = provider
            .GetRequiredService<IOptionsMonitor<WebManiaOptions>>()
            .CurrentValue;

        client.BaseAddress = EnsureTrailingSlash(endpointSelector(options));
        client.Timeout = options.Timeout;
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("DFeHub.Client", "0.1.0"));
    }

    private static Uri EnsureTrailingSlash(Uri value)
    {
        var absoluteUri = value.AbsoluteUri;
        return absoluteUri.EndsWith('/')
            ? value
            : new Uri($"{absoluteUri}/", UriKind.Absolute);
    }

    private sealed class WebManiaRegistrationMarker;
}
