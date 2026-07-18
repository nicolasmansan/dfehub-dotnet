using System.Diagnostics;

namespace DFeHub.Providers.WebMania;

/// <summary>
/// Configures all Webmania API clients registered by <c>AddWebMania</c>.
/// </summary>
public sealed class WebManiaOptions
{
    /// <summary>
    /// Suggested configuration section path.
    /// </summary>
    public const string SectionName = "Fiscal:WebMania";

    /// <summary>
    /// Gets the credentials used by the API 1.0 fiscal endpoints.
    /// </summary>
    public WebManiaApiV1Credentials ApiV1 { get; } = new();

    /// <summary>
    /// Gets the bearer credentials used by the API 2.0 endpoints.
    /// </summary>
    public WebManiaApiV2Credentials ApiV2 { get; } = new();

    /// <summary>
    /// Gets the credentials used by the Validador Fiscal endpoints.
    /// </summary>
    public WebManiaValidatorCredentials Validator { get; } = new();

    /// <summary>
    /// Gets the API 1.0 credentials used exclusively by Gerencial B2B.
    /// </summary>
    public WebManiaApiV1Credentials B2B { get; } = new();

    /// <summary>
    /// Gets the service endpoint configuration.
    /// </summary>
    public WebManiaEndpointOptions Endpoints { get; } = new();

    /// <summary>
    /// Gets or sets the HTTP timeout. The default is 100 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);
}

/// <summary>
/// Contains the four credentials required by Webmania API 1.0.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public sealed class WebManiaApiV1Credentials
{
    /// <summary>
    /// Gets or sets the consumer key.
    /// </summary>
    public string? ConsumerKey { get; set; }

    /// <summary>
    /// Gets or sets the consumer secret.
    /// </summary>
    public string? ConsumerSecret { get; set; }

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the access token secret.
    /// </summary>
    public string? AccessTokenSecret { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var configured =
            !string.IsNullOrWhiteSpace(ConsumerKey) &&
            !string.IsNullOrWhiteSpace(ConsumerSecret) &&
            !string.IsNullOrWhiteSpace(AccessToken) &&
            !string.IsNullOrWhiteSpace(AccessTokenSecret);

        return $"Webmania API 1.0 credentials (configured: {configured})";
    }
}

/// <summary>
/// Contains the bearer credential required by Webmania API 2.0.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public sealed class WebManiaApiV2Credentials
{
    /// <summary>
    /// Gets or sets the bearer access token.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var configured = !string.IsNullOrWhiteSpace(AccessToken);
        return $"Webmania API 2.0 credentials (configured: {configured})";
    }
}

/// <summary>
/// Contains the token required by the Webmania Validador Fiscal API.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public sealed class WebManiaValidatorCredentials
{
    /// <summary>
    /// Gets or sets the value sent in the <c>X-Token</c> header.
    /// </summary>
    public string? Token { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var configured = !string.IsNullOrWhiteSpace(Token);
        return $"Webmania Validador Fiscal credentials (configured: {configured})";
    }
}

/// <summary>
/// Configures the base addresses used to reach Webmania services.
/// </summary>
public sealed class WebManiaEndpointOptions
{
    /// <summary>
    /// Gets or sets the base address for API 1.0.
    /// </summary>
    public Uri ApiV1 { get; set; } = new("https://webmania.com.br/api/");

    /// <summary>
    /// Gets or sets the base address for API 2.0.
    /// </summary>
    public Uri ApiV2 { get; set; } = new("https://api.webmania.com.br/");

    /// <summary>
    /// Gets or sets the base address for Validador Fiscal.
    /// </summary>
    public Uri Validator { get; set; } = new("https://api.webmania.com.br/");
}

