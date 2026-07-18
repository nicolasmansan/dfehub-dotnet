using DFeHub.Exceptions;
using DFeHub.Http;
using DFeHub.Providers.WebMania;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DFeHub.Client.UnitTests;

public sealed class WebManiaDependencyInjectionTests
{
    [Fact]
    public void Delegate_registration_resolves_facade_and_each_product_client()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWebMania(options =>
        {
            options.ApiV1.ConsumerKey = "consumer";
            options.ApiV1.ConsumerSecret = "consumer-secret";
            options.ApiV1.AccessToken = "token";
            options.ApiV1.AccessTokenSecret = "token-secret";
        });

        using var provider = services.BuildServiceProvider();
        var facade = provider.GetRequiredService<IWebManiaClient>();

        Assert.Same(provider.GetRequiredService<INFeClient>(), facade.NFe);
        Assert.Same(provider.GetRequiredService<INFSeClient>(), facade.NFSe);
        Assert.Same(provider.GetRequiredService<ICTeClient>(), facade.CTe);
        Assert.Same(provider.GetRequiredService<IMDFeClient>(), facade.MDFe);
        Assert.Same(provider.GetRequiredService<INFComClient>(), facade.NFCom);
        Assert.Same(provider.GetRequiredService<IDCeClient>(), facade.DCe);
        Assert.Same(
            provider.GetRequiredService<IMonitorFiscalClient>(),
            facade.Monitor);
        Assert.Same(
            provider.GetRequiredService<IValidatorFiscalClient>(),
            facade.Validator);
        Assert.Same(provider.GetRequiredService<IB2BClient>(), facade.B2B);

        Assert.True(facade.NFe.IsConfigured);
        Assert.False(facade.NFSe.IsConfigured);
        Assert.False(facade.CTe.IsConfigured);
        Assert.False(facade.MDFe.IsConfigured);
        Assert.False(facade.NFCom.IsConfigured);
        Assert.False(facade.DCe.IsConfigured);
        Assert.False(facade.Monitor.IsConfigured);
        Assert.False(facade.Validator.IsConfigured);
        Assert.False(facade.B2B.IsConfigured);
    }

    [Fact]
    public void Configuration_registration_binds_all_credential_groups()
    {
        var values = new Dictionary<string, string?>
        {
            ["ApiV1:ConsumerKey"] = "consumer",
            ["ApiV1:ConsumerSecret"] = "consumer-secret",
            ["ApiV1:AccessToken"] = "token",
            ["ApiV1:AccessTokenSecret"] = "token-secret",
            ["ApiV2:AccessToken"] = "bearer",
            ["Validator:Token"] = "validator",
            ["B2B:ConsumerKey"] = "b2b-consumer",
            ["B2B:ConsumerSecret"] = "b2b-consumer-secret",
            ["B2B:AccessToken"] = "b2b-token",
            ["B2B:AccessTokenSecret"] = "b2b-token-secret",
            ["Endpoints:ApiV1"] = "https://localhost:5001/api/",
            ["Endpoints:ApiV2"] = "https://localhost:5002/",
            ["Endpoints:Validator"] = "https://localhost:5003/",
            ["Timeout"] = "00:00:42",
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWebMania(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider
            .GetRequiredService<IOptions<WebManiaOptions>>()
            .Value;
        var facade = provider.GetRequiredService<IWebManiaClient>();

        Assert.Equal("consumer", options.ApiV1.ConsumerKey);
        Assert.Equal("bearer", options.ApiV2.AccessToken);
        Assert.Equal("validator", options.Validator.Token);
        Assert.Equal("b2b-consumer", options.B2B.ConsumerKey);
        Assert.Equal(
            new Uri("https://localhost:5001/api/"),
            options.Endpoints.ApiV1);
        Assert.Equal(
            new Uri("https://localhost:5002/"),
            options.Endpoints.ApiV2);
        Assert.Equal(
            new Uri("https://localhost:5003/"),
            options.Endpoints.Validator);
        Assert.Equal(TimeSpan.FromSeconds(42), options.Timeout);
        Assert.True(facade.NFe.IsConfigured);
        Assert.True(facade.NFSe.IsConfigured);
        Assert.True(facade.Validator.IsConfigured);
        Assert.True(facade.B2B.IsConfigured);
    }

    [Fact]
    public void Configuration_reload_updates_monitored_options()
    {
        var values = new Dictionary<string, string?>
        {
            ["Timeout"] = "00:00:42",
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
        var services = new ServiceCollection();
        services.AddWebMania(configuration);

        using var provider = services.BuildServiceProvider();
        var monitor =
            provider.GetRequiredService<IOptionsMonitor<WebManiaOptions>>();

        Assert.Equal(TimeSpan.FromSeconds(42), monitor.CurrentValue.Timeout);

        configuration["Timeout"] = "00:00:24";
        configuration.Reload();

        Assert.Equal(TimeSpan.FromSeconds(24), monitor.CurrentValue.Timeout);
    }

    [Fact]
    public void Repeated_registration_does_not_duplicate_product_clients()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWebMania(_ => { });
        services.AddWebMania(_ => { });

        using var provider = services.BuildServiceProvider();

        Assert.Single(provider.GetServices<INFeClient>());
        Assert.Single(provider.GetServices<IWebManiaClient>());
    }

    [Fact]
    public void Named_clients_use_expected_endpoints_headers_and_timeout()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWebMania(options =>
        {
            options.Timeout = TimeSpan.FromSeconds(12);
            options.Endpoints.ApiV1 = new Uri("https://v1.example.test/api");
            options.Endpoints.ApiV2 = new Uri("https://v2.example.test");
            options.Endpoints.Validator =
                new Uri("https://validator.example.test/root");
        });

        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();

        using var apiV1 = factory.CreateClient(WebManiaHttpClientNames.ApiV1);
        using var b2b = factory.CreateClient(WebManiaHttpClientNames.B2B);
        using var apiV2 = factory.CreateClient(WebManiaHttpClientNames.ApiV2);
        using var validator =
            factory.CreateClient(WebManiaHttpClientNames.Validator);

        AssertClient(
            apiV1,
            "https://v1.example.test/api/",
            TimeSpan.FromSeconds(12));
        AssertClient(
            b2b,
            "https://v1.example.test/api/",
            TimeSpan.FromSeconds(12));
        AssertClient(
            apiV2,
            "https://v2.example.test/",
            TimeSpan.FromSeconds(12));
        AssertClient(
            validator,
            "https://validator.example.test/root/",
            TimeSpan.FromSeconds(12));
    }

    [Fact]
    public void Invalid_global_transport_options_fail_when_client_is_created()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWebMania(options => options.Timeout = TimeSpan.Zero);

        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();

        Assert.Throws<OptionsValidationException>(
            () => factory.CreateClient(WebManiaHttpClientNames.ApiV2));
    }

    [Fact]
    public async Task Missing_credentials_fail_only_when_corresponding_request_is_sent()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWebMania(_ => { });

        using var provider = services.BuildServiceProvider();
        var facade = provider.GetRequiredService<IWebManiaClient>();
        var factory = provider.GetRequiredService<IHttpClientFactory>();

        Assert.False(facade.NFe.IsConfigured);
        using var client = factory.CreateClient(WebManiaHttpClientNames.ApiV1);

        await Assert.ThrowsAsync<WebManiaConfigurationException>(
            () => client.GetAsync("1/nfe/consulta/"));
    }

    [Fact]
    public void Null_arguments_are_rejected()
    {
        IServiceCollection? services = null;
        var validServices = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(
            () => services!.AddWebMania(_ => { }));
        Assert.Throws<ArgumentNullException>(
            () => validServices.AddWebMania((Action<WebManiaOptions>)null!));
        Assert.Throws<ArgumentNullException>(
            () => validServices.AddWebMania((IConfiguration)null!));
    }

    private static void AssertClient(
        HttpClient client,
        string expectedAddress,
        TimeSpan expectedTimeout)
    {
        var expectedSdkVersion =
            typeof(IWebManiaClient).Assembly.GetName().Version?.ToString(3);

        Assert.Equal(new Uri(expectedAddress), client.BaseAddress);
        Assert.Equal(expectedTimeout, client.Timeout);
        Assert.Contains(
            client.DefaultRequestHeaders.Accept,
            value => value.MediaType == "application/json");
        Assert.Contains(
            client.DefaultRequestHeaders.UserAgent,
            value =>
                value.Product?.Name == "DFeHub.Client" &&
                value.Product.Version == expectedSdkVersion);
    }
}
