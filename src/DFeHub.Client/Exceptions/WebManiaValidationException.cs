using DFeHub.Validation;

namespace DFeHub.Exceptions;

/// <summary>
/// Represents one or more deterministic request validation failures.
/// </summary>
public sealed class WebManiaValidationException : WebManiaException
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="WebManiaValidationException"/> class.
    /// </summary>
    public WebManiaValidationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="WebManiaValidationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public WebManiaValidationException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="WebManiaValidationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this error.</param>
    public WebManiaValidationException(
        string? message,
        Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="WebManiaValidationException"/> class.
    /// </summary>
    /// <param name="failures">The validation failures.</param>
    public WebManiaValidationException(
        IReadOnlyList<WebManiaValidationFailure> failures)
        : base("The Webmania request is invalid.")
    {
        Failures = failures ?? throw new ArgumentNullException(nameof(failures));
    }

    /// <summary>
    /// Gets the validation failures.
    /// </summary>
    public IReadOnlyList<WebManiaValidationFailure> Failures { get; } = [];
}

