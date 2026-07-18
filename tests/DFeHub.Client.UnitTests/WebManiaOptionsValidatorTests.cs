using DFeHub.Providers.WebMania;

namespace DFeHub.Client.UnitTests;

public sealed class WebManiaOptionsValidatorTests
{
    private readonly WebManiaOptionsValidator validator = new();

    [Fact]
    public void Defaults_are_valid()
    {
        var result = validator.Validate(null, new WebManiaOptions());

        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Loopback_http_is_allowed_for_local_tests()
    {
        var options = new WebManiaOptions();
        options.Endpoints.ApiV1 = new Uri("http://localhost:5000/api");
        options.Endpoints.ApiV2 = new Uri("http://127.0.0.1:5001");
        options.Endpoints.Validator = new Uri("http://[::1]:5002");

        var result = validator.Validate(null, options);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Invalid_endpoints_and_timeout_are_reported_together()
    {
        var options = new WebManiaOptions
        {
            Timeout = TimeSpan.Zero,
        };
        options.Endpoints.ApiV1 = new Uri("/relative", UriKind.Relative);
        options.Endpoints.ApiV2 = new Uri("http://example.com");
        options.Endpoints.Validator = null!;

        var result = validator.Validate(null, options);
        Assert.NotNull(result.Failures);
        var failures = result.Failures.ToArray();

        Assert.False(result.Succeeded);
        Assert.Contains(failures, value => value.Contains("Endpoints:ApiV1"));
        Assert.Contains(failures, value => value.Contains("Endpoints:ApiV2"));
        Assert.Contains(
            failures,
            value => value.Contains("Endpoints:Validator"));
        Assert.Contains(failures, value => value.Contains("Timeout"));
    }

    [Fact]
    public void Timeout_longer_than_ten_minutes_is_rejected()
    {
        var options = new WebManiaOptions
        {
            Timeout = TimeSpan.FromMinutes(11),
        };

        var result = validator.Validate(null, options);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public void Null_options_are_rejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => validator.Validate(null, null!));
    }
}
