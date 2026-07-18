using System.Net;
using DFeHub.Exceptions;
using DFeHub.Validation;

namespace DFeHub.Client.UnitTests;

public sealed class WebManiaExceptionTests
{
    [Fact]
    public void Standard_exception_constructors_preserve_messages_and_inner_errors()
    {
        var inner = new InvalidOperationException("inner");

        WebManiaException[] exceptions =
        [
            new WebManiaException(),
            new WebManiaException("base"),
            new WebManiaException("base-inner", inner),
            new WebManiaConfigurationException(),
            new WebManiaConfigurationException("configuration"),
            new WebManiaConfigurationException("configuration-inner", inner),
            new WebManiaValidationException(),
            new WebManiaValidationException("validation"),
            new WebManiaValidationException("validation-inner", inner),
            new WebManiaTransportException(),
            new WebManiaTransportException("transport"),
            new WebManiaTransportException("transport-inner", inner),
        ];

        Assert.Equal(12, exceptions.Length);
        Assert.Same(inner, exceptions[2].InnerException);
        Assert.Same(inner, exceptions[5].InnerException);
        Assert.Same(inner, exceptions[8].InnerException);
        Assert.Same(inner, exceptions[11].InnerException);
    }

    [Fact]
    public void Validation_exception_preserves_structured_failures()
    {
        WebManiaValidationFailure[] failures =
        [
            new("cliente.cpf", "CPF inválido.", "FORMAT"),
        ];

        var exception = new WebManiaValidationException(failures);

        Assert.Same(failures, exception.Failures);
        Assert.Equal("FORMAT", Assert.Single(exception.Failures).Code);
        Assert.Throws<ArgumentNullException>(
            () => new WebManiaValidationException(
                (IReadOnlyList<WebManiaValidationFailure>)null!));
    }

    [Fact]
    public void Api_exception_hierarchy_preserves_safe_metadata()
    {
        WebManiaApiError[] errors = [new("Falha.", "CODE", "field")];
        var authentication = new WebManiaAuthenticationException(
            "Unauthorized",
            HttpStatusCode.Unauthorized,
            "NFe",
            "Issue",
            "request-id",
            errors);
        var rateLimit = new WebManiaRateLimitException(
            "Rate",
            HttpStatusCode.TooManyRequests,
            "NFe",
            "Query",
            TimeSpan.FromSeconds(5),
            errors: errors);
        var server = new WebManiaServerException(
            "Server",
            HttpStatusCode.InternalServerError,
            "NFe",
            "Query",
            errors: errors);
        var generic = new WebManiaApiException(
            "Generic",
            HttpStatusCode.BadRequest,
            "NFe",
            "Query");

        Assert.Equal("request-id", authentication.RequestId);
        Assert.Same(errors, authentication.Errors);
        Assert.Equal(TimeSpan.FromSeconds(5), rateLimit.RetryAfter);
        Assert.Same(errors, rateLimit.Errors);
        Assert.Same(errors, server.Errors);
        Assert.Empty(generic.Errors);
    }
}
