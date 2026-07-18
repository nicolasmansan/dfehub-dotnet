namespace DFeHub.Exceptions;

/// <summary>
/// Base class for errors produced by DFeHub's Webmania integration.
/// </summary>
public class WebManiaException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebManiaException"/> class.
    /// </summary>
    public WebManiaException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebManiaException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public WebManiaException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebManiaException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this error.</param>
    public WebManiaException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

