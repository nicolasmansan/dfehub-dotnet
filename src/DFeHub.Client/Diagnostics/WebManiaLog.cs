using Microsoft.Extensions.Logging;

namespace DFeHub.Diagnostics;

internal static partial class WebManiaLog
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Debug,
        Message = "Webmania {Product}/{Operation} returned HTTP {StatusCode} in {ElapsedMilliseconds} ms.")]
    internal static partial void RequestCompleted(
        ILogger logger,
        string product,
        string operation,
        int statusCode,
        double elapsedMilliseconds);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "Webmania {Product}/{Operation} failed during HTTP transport.")]
    internal static partial void TransportFailed(
        ILogger logger,
        Exception exception,
        string product,
        string operation);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Warning,
        Message = "Webmania {Product}/{Operation} returned a response that could not be decoded.")]
    internal static partial void ResponseDecodingFailed(
        ILogger logger,
        Exception exception,
        string product,
        string operation);
}

