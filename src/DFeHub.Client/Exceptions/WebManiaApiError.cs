namespace DFeHub.Exceptions;

/// <summary>
/// Describes a structured error returned by a Webmania endpoint.
/// </summary>
/// <param name="Message">The provider error message.</param>
/// <param name="Code">An optional provider error code.</param>
/// <param name="Field">An optional request field associated with the error.</param>
public sealed record WebManiaApiError(
    string Message,
    string? Code = null,
    string? Field = null);

