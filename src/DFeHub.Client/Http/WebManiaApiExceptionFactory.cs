using System.Buffers;
using System.Net;
using System.Text.Json;
using DFeHub.Exceptions;
using DFeHub.Serialization;

namespace DFeHub.Http;

internal static class WebManiaApiExceptionFactory
{
    private const int MaximumErrorBodyBytes = 64 * 1024;

    internal static async Task<WebManiaApiException> CreateAsync(
        HttpResponseMessage response,
        string product,
        string operation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(response);

        var body = await ReadLimitedBodyAsync(
                response.Content,
                cancellationToken)
            .ConfigureAwait(false);

        var errors = new List<WebManiaApiError>();
        var message = TryParsePayload(body, errors);

        if (string.IsNullOrWhiteSpace(message))
        {
            message =
                response.ReasonPhrase ??
                $"Webmania returned HTTP {(int)response.StatusCode}.";
        }

        var requestId = GetRequestId(response);

        return response.StatusCode switch
        {
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden =>
                new WebManiaAuthenticationException(
                    message,
                    response.StatusCode,
                    product,
                    operation,
                    requestId,
                    errors),
            HttpStatusCode.TooManyRequests =>
                new WebManiaRateLimitException(
                    message,
                    response.StatusCode,
                    product,
                    operation,
                    GetRetryAfter(response),
                    requestId,
                    errors),
            >= HttpStatusCode.InternalServerError =>
                new WebManiaServerException(
                    message,
                    response.StatusCode,
                    product,
                    operation,
                    requestId,
                    errors),
            _ => new WebManiaApiException(
                message,
                response.StatusCode,
                product,
                operation,
                requestId,
                errors),
        };
    }

    private static string? TryParsePayload(
        ReadOnlySpan<byte> body,
        List<WebManiaApiError> errors)
    {
        if (body.IsEmpty)
        {
            return null;
        }

        try
        {
            var payload = JsonSerializer.Deserialize(
                body,
                WebManiaJsonSerializerContext.Default.WebManiaErrorPayload);

            if (payload is null)
            {
                return null;
            }

            AddErrors(payload.Error, errors);
            AddErrors(payload.Errors, errors);

            var message = payload.Message ?? payload.Mensagem;
            var code = payload.Code ?? payload.Codigo;

            if (!string.IsNullOrWhiteSpace(message) &&
                errors.All(error => error.Message != message))
            {
                errors.Insert(0, new WebManiaApiError(message, code));
            }

            return message ?? errors.FirstOrDefault()?.Message;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static void AddErrors(
        JsonElement element,
        List<WebManiaApiError> errors)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                AddError(errors, element.GetString());
                break;
            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                {
                    AddErrors(item, errors);
                }

                break;
            case JsonValueKind.Object:
                AddObjectError(element, errors);
                break;
        }
    }

    private static void AddObjectError(
        JsonElement element,
        List<WebManiaApiError> errors)
    {
        var message =
            GetString(element, "message") ??
            GetString(element, "mensagem") ??
            GetString(element, "error") ??
            GetString(element, "erro");
        var code =
            GetString(element, "code") ??
            GetString(element, "codigo");
        var field =
            GetString(element, "field") ??
            GetString(element, "campo");

        if (!string.IsNullOrWhiteSpace(message))
        {
            errors.Add(new WebManiaApiError(message, code, field));
            return;
        }

        foreach (var property in element.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.String)
            {
                AddError(errors, property.Value.GetString(), property.Name);
            }
            else
            {
                AddErrors(property.Value, errors);
            }
        }
    }

    private static string? GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) &&
            property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }

    private static void AddError(
        List<WebManiaApiError> errors,
        string? message,
        string? field = null)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            errors.Add(new WebManiaApiError(message, Field: field));
        }
    }

    private static string? GetRequestId(HttpResponseMessage response)
    {
        string[] headerNames =
        [
            "X-Request-Id",
            "Request-Id",
            "X-Correlation-Id",
        ];

        foreach (var headerName in headerNames)
        {
            if (response.Headers.TryGetValues(headerName, out var values))
            {
                return values.FirstOrDefault();
            }
        }

        return null;
    }

    private static TimeSpan? GetRetryAfter(HttpResponseMessage response)
    {
        var retryAfter = response.Headers.RetryAfter;
        if (retryAfter?.Delta is not null)
        {
            return retryAfter.Delta;
        }

        if (retryAfter?.Date is null)
        {
            return null;
        }

        var delay = retryAfter.Date.Value - DateTimeOffset.UtcNow;
        return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
    }

    private static async Task<byte[]> ReadLimitedBodyAsync(
        HttpContent content,
        CancellationToken cancellationToken)
    {
        await using var stream = await content
            .ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);

        var buffer = ArrayPool<byte>.Shared.Rent(MaximumErrorBodyBytes);
        try
        {
            var totalRead = 0;

            while (totalRead < MaximumErrorBodyBytes)
            {
                var read = await stream
                    .ReadAsync(
                        buffer.AsMemory(
                            totalRead,
                            MaximumErrorBodyBytes - totalRead),
                        cancellationToken)
                    .ConfigureAwait(false);

                if (read == 0)
                {
                    break;
                }

                totalRead += read;
            }

            return buffer.AsSpan(0, totalRead).ToArray();
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer, clearArray: true);
        }
    }
}

