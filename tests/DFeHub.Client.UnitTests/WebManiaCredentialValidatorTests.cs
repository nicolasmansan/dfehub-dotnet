using DFeHub.Exceptions;
using DFeHub.Providers.WebMania;

namespace DFeHub.Client.UnitTests;

public sealed class WebManiaCredentialValidatorTests
{
    [Theory]
    [InlineData(WebManiaCredentialGroup.ApiV1)]
    [InlineData(WebManiaCredentialGroup.ApiV2)]
    [InlineData(WebManiaCredentialGroup.Validator)]
    [InlineData(WebManiaCredentialGroup.B2B)]
    internal void Complete_group_is_configured(WebManiaCredentialGroup group)
    {
        var options = WebManiaOptionsTests.CreateFullyConfiguredOptions();

        Assert.True(WebManiaCredentialValidator.IsConfigured(options, group));
        WebManiaCredentialValidator.EnsureConfigured(options, group);
    }

    [Theory]
    [InlineData(WebManiaCredentialGroup.ApiV1)]
    [InlineData(WebManiaCredentialGroup.ApiV2)]
    [InlineData(WebManiaCredentialGroup.Validator)]
    [InlineData(WebManiaCredentialGroup.B2B)]
    internal void Empty_group_is_not_configured(WebManiaCredentialGroup group)
    {
        var options = new WebManiaOptions();

        Assert.False(WebManiaCredentialValidator.IsConfigured(options, group));
    }

    [Fact]
    public void Missing_fields_are_named_without_exposing_configured_values()
    {
        var options = new WebManiaOptions();
        options.ApiV1.ConsumerKey = "must-not-leak";

        var exception = Assert.Throws<WebManiaConfigurationException>(
            () => WebManiaCredentialValidator.EnsureConfigured(
                options,
                WebManiaCredentialGroup.ApiV1));

        Assert.Contains("ApiV1", exception.Message);
        Assert.Contains("ConsumerSecret", exception.Message);
        Assert.Contains("AccessToken", exception.Message);
        Assert.Contains("AccessTokenSecret", exception.Message);
        Assert.DoesNotContain("must-not-leak", exception.Message);
    }

    [Fact]
    public void Unknown_group_is_rejected_without_values()
    {
        var options = WebManiaOptionsTests.CreateFullyConfiguredOptions();
        var group = (WebManiaCredentialGroup)999;

        Assert.False(WebManiaCredentialValidator.IsConfigured(options, group));

        var exception = Assert.Throws<WebManiaConfigurationException>(
            () => WebManiaCredentialValidator.EnsureConfigured(options, group));

        Assert.Contains("Unknown", exception.Message);
    }

    [Fact]
    public void Null_options_are_rejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => WebManiaCredentialValidator.IsConfigured(
                null!,
                WebManiaCredentialGroup.ApiV1));
        Assert.Throws<ArgumentNullException>(
            () => WebManiaCredentialValidator.EnsureConfigured(
                null!,
                WebManiaCredentialGroup.ApiV1));
    }
}

