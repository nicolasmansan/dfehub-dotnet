using System.Text.Json;
using DFeHub.Http.Authentication;
using DFeHub.Providers.WebMania;
using DFeHub.Serialization;

namespace DFeHub.Client.ContractTests;

public sealed class FoundationContractTests
{
    [Fact]
    public void Api_roots_match_official_documentation()
    {
        var options = new WebManiaOptions();

        Assert.Equal(
            "https://webmania.com.br/api/",
            options.Endpoints.ApiV1.AbsoluteUri);
        Assert.Equal(
            "https://api.webmania.com.br/",
            options.Endpoints.ApiV2.AbsoluteUri);
        Assert.Equal(
            "https://api.webmania.com.br/",
            options.Endpoints.Validator.AbsoluteUri);
    }

    [Fact]
    public void Authentication_header_names_match_official_contract()
    {
        Assert.Equal(
            "X-Consumer-Key",
            ApiV1AuthenticationHandler.ConsumerKeyHeader);
        Assert.Equal(
            "X-Consumer-Secret",
            ApiV1AuthenticationHandler.ConsumerSecretHeader);
        Assert.Equal(
            "X-Access-Token",
            ApiV1AuthenticationHandler.AccessTokenHeader);
        Assert.Equal(
            "X-Access-Token-Secret",
            ApiV1AuthenticationHandler.AccessTokenSecretHeader);
        Assert.Equal(
            "X-Token",
            ValidatorAuthenticationHandler.TokenHeader);
    }

    [Fact]
    public void Foundation_error_payload_is_source_generated()
    {
        const string json =
            """
            {
              "error": "Falha de contrato.",
              "message": "Requisição inválida.",
              "code": "INVALID"
            }
            """;

        var result = JsonSerializer.Deserialize(
            json,
            WebManiaJsonSerializerContext.Default.WebManiaErrorPayload);

        Assert.NotNull(result);
        Assert.Equal("Falha de contrato.", result.Error.GetString());
        Assert.Equal("Requisição inválida.", result.Message);
        Assert.Equal("INVALID", result.Code);
    }

    [Fact]
    public void Assembly_and_public_facade_names_are_stable()
    {
        var assembly = typeof(IWebManiaClient).Assembly;
        var propertyNames = typeof(IWebManiaClient)
            .GetProperties()
            .Select(property => property.Name)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal("DFeHub", assembly.GetName().Name);
        Assert.Equal(
            [
                "B2B",
                "CTe",
                "DCe",
                "MDFe",
                "Monitor",
                "NFCom",
                "NFSe",
                "NFe",
                "Validator",
            ],
            propertyNames);
    }
}
