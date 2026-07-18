namespace DFeHub.Validation;

/// <summary>
/// Describes a request validation failure.
/// </summary>
/// <param name="PropertyPath">The request property path.</param>
/// <param name="Message">The validation message.</param>
/// <param name="Code">An optional stable validation code.</param>
public sealed record WebManiaValidationFailure(
    string PropertyPath,
    string Message,
    string? Code = null);

