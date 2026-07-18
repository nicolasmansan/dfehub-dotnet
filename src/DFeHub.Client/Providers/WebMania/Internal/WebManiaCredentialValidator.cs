using DFeHub.Exceptions;

namespace DFeHub.Providers.WebMania;

internal static class WebManiaCredentialValidator
{
    internal static bool IsConfigured(
        WebManiaOptions options,
        WebManiaCredentialGroup group)
    {
        ArgumentNullException.ThrowIfNull(options);

        return group switch
        {
            WebManiaCredentialGroup.ApiV1 => IsConfigured(options.ApiV1),
            WebManiaCredentialGroup.ApiV2 =>
                !string.IsNullOrWhiteSpace(options.ApiV2.AccessToken),
            WebManiaCredentialGroup.Validator =>
                !string.IsNullOrWhiteSpace(options.Validator.Token),
            WebManiaCredentialGroup.B2B => IsConfigured(options.B2B),
            _ => false,
        };
    }

    internal static void EnsureConfigured(
        WebManiaOptions options,
        WebManiaCredentialGroup group)
    {
        ArgumentNullException.ThrowIfNull(options);

        var missing = GetMissingFields(options, group);
        if (missing.Count == 0)
        {
            return;
        }

        var groupName = group switch
        {
            WebManiaCredentialGroup.ApiV1 => nameof(WebManiaOptions.ApiV1),
            WebManiaCredentialGroup.ApiV2 => nameof(WebManiaOptions.ApiV2),
            WebManiaCredentialGroup.Validator => nameof(WebManiaOptions.Validator),
            WebManiaCredentialGroup.B2B => nameof(WebManiaOptions.B2B),
            _ => "Unknown",
        };

        throw new WebManiaConfigurationException(
            $"Webmania credential group '{groupName}' is incomplete. " +
            $"Missing: {string.Join(", ", missing)}.");
    }

    private static List<string> GetMissingFields(
        WebManiaOptions options,
        WebManiaCredentialGroup group)
    {
        var missing = new List<string>();

        switch (group)
        {
            case WebManiaCredentialGroup.ApiV1:
                AddMissingApiV1Fields(options.ApiV1, missing);
                break;
            case WebManiaCredentialGroup.ApiV2:
                AddIfMissing(
                    options.ApiV2.AccessToken,
                    nameof(WebManiaApiV2Credentials.AccessToken),
                    missing);
                break;
            case WebManiaCredentialGroup.Validator:
                AddIfMissing(
                    options.Validator.Token,
                    nameof(WebManiaValidatorCredentials.Token),
                    missing);
                break;
            case WebManiaCredentialGroup.B2B:
                AddMissingApiV1Fields(options.B2B, missing);
                break;
            default:
                missing.Add("credential group");
                break;
        }

        return missing;
    }

    private static bool IsConfigured(WebManiaApiV1Credentials credentials)
    {
        return
            !string.IsNullOrWhiteSpace(credentials.ConsumerKey) &&
            !string.IsNullOrWhiteSpace(credentials.ConsumerSecret) &&
            !string.IsNullOrWhiteSpace(credentials.AccessToken) &&
            !string.IsNullOrWhiteSpace(credentials.AccessTokenSecret);
    }

    private static void AddMissingApiV1Fields(
        WebManiaApiV1Credentials credentials,
        List<string> missing)
    {
        AddIfMissing(
            credentials.ConsumerKey,
            nameof(WebManiaApiV1Credentials.ConsumerKey),
            missing);
        AddIfMissing(
            credentials.ConsumerSecret,
            nameof(WebManiaApiV1Credentials.ConsumerSecret),
            missing);
        AddIfMissing(
            credentials.AccessToken,
            nameof(WebManiaApiV1Credentials.AccessToken),
            missing);
        AddIfMissing(
            credentials.AccessTokenSecret,
            nameof(WebManiaApiV1Credentials.AccessTokenSecret),
            missing);
    }

    private static void AddIfMissing(
        string? value,
        string field,
        List<string> missing)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            missing.Add(field);
        }
    }
}

