using DFeHub.Providers.WebMania;

namespace DFeHub.Client.UnitTests;

public sealed class WebManiaOptionsTests
{
    [Fact]
    public void Defaults_match_official_service_roots()
    {
        var options = new WebManiaOptions();

        Assert.Equal(
            new Uri("https://webmania.com.br/api/"),
            options.Endpoints.ApiV1);
        Assert.Equal(
            new Uri("https://api.webmania.com.br/"),
            options.Endpoints.ApiV2);
        Assert.Equal(
            new Uri("https://api.webmania.com.br/"),
            options.Endpoints.Validator);
        Assert.Equal(TimeSpan.FromSeconds(100), options.Timeout);
        Assert.Equal("Fiscal:WebMania", WebManiaOptions.SectionName);
    }

    [Fact]
    public void Credential_string_representations_never_include_secrets()
    {
        var options = CreateFullyConfiguredOptions();

        var representations = new[]
        {
            options.ApiV1.ToString(),
            options.ApiV2.ToString(),
            options.Validator.ToString(),
            options.B2B.ToString(),
        };

        Assert.All(
            representations,
            value =>
            {
                Assert.Contains("configured: True", value);
                Assert.DoesNotContain("consumer-secret", value);
                Assert.DoesNotContain("access-secret", value);
                Assert.DoesNotContain("bearer-secret", value);
                Assert.DoesNotContain("validator-secret", value);
            });
    }

    [Fact]
    public void Empty_credential_string_representations_report_not_configured()
    {
        var options = new WebManiaOptions();

        Assert.Contains("configured: False", options.ApiV1.ToString());
        Assert.Contains("configured: False", options.ApiV2.ToString());
        Assert.Contains("configured: False", options.Validator.ToString());
        Assert.Contains("configured: False", options.B2B.ToString());
    }

    internal static WebManiaOptions CreateFullyConfiguredOptions()
    {
        var options = new WebManiaOptions();

        options.ApiV1.ConsumerKey = "consumer-key";
        options.ApiV1.ConsumerSecret = "consumer-secret";
        options.ApiV1.AccessToken = "access-token";
        options.ApiV1.AccessTokenSecret = "access-secret";
        options.ApiV2.AccessToken = "bearer-secret";
        options.Validator.Token = "validator-secret";
        options.B2B.ConsumerKey = "b2b-consumer-key";
        options.B2B.ConsumerSecret = "b2b-consumer-secret";
        options.B2B.AccessToken = "b2b-access-token";
        options.B2B.AccessTokenSecret = "b2b-access-secret";

        return options;
    }
}

