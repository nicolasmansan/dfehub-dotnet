namespace DFeHub.Exceptions;

/// <summary>
/// Represents a failure while transporting or decoding an HTTP response.
/// </summary>
public sealed class WebManiaTransportException : WebManiaException
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="WebManiaTransportException"/> class.
    /// </summary>
    public WebManiaTransportException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="WebManiaTransportException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public WebManiaTransportException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="WebManiaTransportException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this error.</param>
    public WebManiaTransportException(
        string? message,
        Exception? innerException)
        : base(message, innerException)
    {
    }
}

