using System.Net;

namespace DFeHub.Exceptions;

/// <summary>
/// Represents a non-successful response returned by a Webmania API.
/// </summary>
public class WebManiaApiException : WebManiaException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebManiaApiException"/> class.
    /// </summary>
    /// <param name="message">The provider error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="product">The Webmania product.</param>
    /// <param name="operation">The attempted operation.</param>
    /// <param name="requestId">An optional provider request identifier.</param>
    /// <param name="errors">Structured provider errors.</param>
    public WebManiaApiException(
        string message,
        HttpStatusCode statusCode,
        string product,
        string operation,
        string? requestId = null,
        IReadOnlyList<WebManiaApiError>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Product = product;
        Operation = operation;
        RequestId = requestId;
        Errors = errors ?? [];
    }

    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the Webmania product associated with the response.
    /// </summary>
    public string Product { get; }

    /// <summary>
    /// Gets the attempted operation.
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Gets the provider request identifier, when available.
    /// </summary>
    public string? RequestId { get; }

    /// <summary>
    /// Gets structured errors returned by the provider.
    /// </summary>
    public IReadOnlyList<WebManiaApiError> Errors { get; }
}

/// <summary>
/// Represents an authentication or authorization failure.
/// </summary>
public sealed class WebManiaAuthenticationException : WebManiaApiException
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="WebManiaAuthenticationException"/> class.
    /// </summary>
    public WebManiaAuthenticationException(
        string message,
        HttpStatusCode statusCode,
        string product,
        string operation,
        string? requestId = null,
        IReadOnlyList<WebManiaApiError>? errors = null)
        : base(message, statusCode, product, operation, requestId, errors)
    {
    }
}

/// <summary>
/// Represents an API rate limit response.
/// </summary>
public sealed class WebManiaRateLimitException : WebManiaApiException
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="WebManiaRateLimitException"/> class.
    /// </summary>
    public WebManiaRateLimitException(
        string message,
        HttpStatusCode statusCode,
        string product,
        string operation,
        TimeSpan? retryAfter = null,
        string? requestId = null,
        IReadOnlyList<WebManiaApiError>? errors = null)
        : base(message, statusCode, product, operation, requestId, errors)
    {
        RetryAfter = retryAfter;
    }

    /// <summary>
    /// Gets the provider-suggested delay before another attempt.
    /// </summary>
    public TimeSpan? RetryAfter { get; }
}

/// <summary>
/// Represents a server-side Webmania failure.
/// </summary>
public sealed class WebManiaServerException : WebManiaApiException
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="WebManiaServerException"/> class.
    /// </summary>
    public WebManiaServerException(
        string message,
        HttpStatusCode statusCode,
        string product,
        string operation,
        string? requestId = null,
        IReadOnlyList<WebManiaApiError>? errors = null)
        : base(message, statusCode, product, operation, requestId, errors)
    {
    }
}

