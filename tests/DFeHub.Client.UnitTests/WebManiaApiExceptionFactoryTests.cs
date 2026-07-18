using System.Net;
using System.Net.Http.Headers;
using System.Text;
using DFeHub.Exceptions;
using DFeHub.Http;

namespace DFeHub.Client.UnitTests;

public sealed class WebManiaApiExceptionFactoryTests
{
    [Fact]
    public async Task String_error_is_mapped_to_structured_api_exception()
    {
        using var response = JsonResponse(
            HttpStatusCode.BadRequest,
            """{"error":"Documento inválido."}""");
        response.Headers.Add("X-Request-Id", "request-123");

        var exception = await WebManiaApiExceptionFactory.CreateAsync(
            response,
            "NFe",
            "Issue",
            CancellationToken.None);

        Assert.IsType<WebManiaApiException>(exception);
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Documento inválido.", exception.Message);
        Assert.Equal("NFe", exception.Product);
        Assert.Equal("Issue", exception.Operation);
        Assert.Equal("request-123", exception.RequestId);
        Assert.Equal("Documento inválido.", Assert.Single(exception.Errors).Message);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    public async Task Authentication_statuses_have_specific_exception(
        HttpStatusCode statusCode)
    {
        using var response = JsonResponse(
            statusCode,
            """{"message":"Credenciais inválidas.","code":"AUTH"}""");

        var exception = await WebManiaApiExceptionFactory.CreateAsync(
            response,
            "NFSe",
            "Query",
            CancellationToken.None);

        var authentication =
            Assert.IsType<WebManiaAuthenticationException>(exception);
        Assert.Equal("Credenciais inválidas.", authentication.Message);
        Assert.Equal("AUTH", Assert.Single(authentication.Errors).Code);
    }

    [Fact]
    public async Task Rate_limit_preserves_retry_after_delta()
    {
        using var response = JsonResponse(
            HttpStatusCode.TooManyRequests,
            """{"error":{"message":"Limite excedido.","code":"RATE"}}""");
        response.Headers.RetryAfter = new RetryConditionHeaderValue(
            TimeSpan.FromSeconds(15));

        var exception = await WebManiaApiExceptionFactory.CreateAsync(
            response,
            "Validator",
            "Validate",
            CancellationToken.None);

        var rateLimit = Assert.IsType<WebManiaRateLimitException>(exception);
        Assert.Equal(TimeSpan.FromSeconds(15), rateLimit.RetryAfter);
        Assert.Equal("RATE", Assert.Single(rateLimit.Errors).Code);
    }

    [Fact]
    public async Task Rate_limit_supports_retry_after_date()
    {
        using var response = JsonResponse(
            HttpStatusCode.TooManyRequests,
            """{"error":"Aguarde."}""");
        response.Headers.RetryAfter = new RetryConditionHeaderValue(
            DateTimeOffset.UtcNow.AddSeconds(30));

        var exception = await WebManiaApiExceptionFactory.CreateAsync(
            response,
            "Validator",
            "Validate",
            CancellationToken.None);

        var rateLimit = Assert.IsType<WebManiaRateLimitException>(exception);
        Assert.NotNull(rateLimit.RetryAfter);
        Assert.InRange(
            rateLimit.RetryAfter.Value,
            TimeSpan.FromSeconds(20),
            TimeSpan.FromSeconds(30));
    }

    [Fact]
    public async Task Past_retry_after_date_is_normalized_to_zero()
    {
        using var response = JsonResponse(
            HttpStatusCode.TooManyRequests,
            """{"error":"Aguarde."}""");
        response.Headers.RetryAfter = new RetryConditionHeaderValue(
            DateTimeOffset.UtcNow.AddMinutes(-1));

        var exception = await WebManiaApiExceptionFactory.CreateAsync(
            response,
            "Validator",
            "Validate",
            CancellationToken.None);

        Assert.Equal(
            TimeSpan.Zero,
            Assert.IsType<WebManiaRateLimitException>(exception).RetryAfter);
    }

    [Theory]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.BadGateway)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public async Task Server_statuses_have_specific_exception(
        HttpStatusCode statusCode)
    {
        using var response = JsonResponse(
            statusCode,
            """{"mensagem":"Falha temporária.","codigo":"SERVER"}""");

        var exception = await WebManiaApiExceptionFactory.CreateAsync(
            response,
            "CTe",
            "Issue",
            CancellationToken.None);

        var server = Assert.IsType<WebManiaServerException>(exception);
        Assert.Equal("Falha temporária.", server.Message);
        Assert.Equal("SERVER", Assert.Single(server.Errors).Code);
    }

    [Fact]
    public async Task Error_array_preserves_message_code_and_field()
    {
        using var response = JsonResponse(
            HttpStatusCode.UnprocessableEntity,
            """
            {
              "errors": [
                {"message":"Campo obrigatório.","code":"REQUIRED","field":"tomador.cpf"},
                {"mensagem":"Formato inválido.","codigo":"FORMAT","campo":"prestador.cnpj"}
              ]
            }
            """);

        var exception = await WebManiaApiExceptionFactory.CreateAsync(
            response,
            "NFSe",
            "Issue",
            CancellationToken.None);

        Assert.Equal(2, exception.Errors.Count);
        Assert.Equal("tomador.cpf", exception.Errors[0].Field);
        Assert.Equal("REQUIRED", exception.Errors[0].Code);
        Assert.Equal("prestador.cnpj", exception.Errors[1].Field);
        Assert.Equal("FORMAT", exception.Errors[1].Code);
    }

    [Fact]
    public async Task Field_dictionary_is_flattened_without_raw_body()
    {
        using var response = JsonResponse(
            HttpStatusCode.BadRequest,
            """
            {
              "error": {
                "cliente.cpf": "CPF inválido.",
                "produtos": ["Informe ao menos um produto."]
              }
            }
            """);

        var exception = await WebManiaApiExceptionFactory.CreateAsync(
            response,
            "NFe",
            "Issue",
            CancellationToken.None);

        Assert.Equal(2, exception.Errors.Count);
        Assert.Contains(
            exception.Errors,
            error =>
                error.Field == "cliente.cpf" &&
                error.Message == "CPF inválido.");
        Assert.Contains(
            exception.Errors,
            error => error.Message == "Informe ao menos um produto.");
    }

    [Fact]
    public async Task Malformed_or_empty_body_falls_back_to_reason_phrase()
    {
        using var malformed = new HttpResponseMessage(HttpStatusCode.BadGateway)
        {
            Content = new StringContent("not-json", Encoding.UTF8, "text/plain"),
            ReasonPhrase = "Gateway indisponível",
        };
        using var empty = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new ByteArrayContent([]),
            ReasonPhrase = "Requisição inválida",
        };

        var malformedException = await WebManiaApiExceptionFactory.CreateAsync(
            malformed,
            "NFe",
            "Query",
            CancellationToken.None);
        var emptyException = await WebManiaApiExceptionFactory.CreateAsync(
            empty,
            "NFe",
            "Query",
            CancellationToken.None);

        Assert.Equal("Gateway indisponível", malformedException.Message);
        Assert.Equal("Requisição inválida", emptyException.Message);
        Assert.Empty(malformedException.Errors);
        Assert.Empty(emptyException.Errors);
    }

    [Theory]
    [InlineData("Request-Id")]
    [InlineData("X-Correlation-Id")]
    public async Task Alternative_request_id_headers_are_supported(
        string headerName)
    {
        using var response = JsonResponse(
            HttpStatusCode.BadRequest,
            """{"error":"Falha."}""");
        response.Headers.Add(headerName, "correlation-456");

        var exception = await WebManiaApiExceptionFactory.CreateAsync(
            response,
            "NFe",
            "Query",
            CancellationToken.None);

        Assert.Equal("correlation-456", exception.RequestId);
    }

    [Fact]
    public async Task Null_response_is_rejected()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => WebManiaApiExceptionFactory.CreateAsync(
                null!,
                "NFe",
                "Query",
                CancellationToken.None));
    }

    private static HttpResponseMessage JsonResponse(
        HttpStatusCode statusCode,
        string json)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };
    }
}

