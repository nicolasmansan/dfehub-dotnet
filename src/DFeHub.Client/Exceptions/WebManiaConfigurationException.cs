namespace DFeHub.Exceptions;

/// <summary>
/// Represents invalid or missing Webmania client configuration.
/// </summary>
public sealed class WebManiaConfigurationException : WebManiaException
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="WebManiaConfigurationException"/> class.
    /// </summary>
    public WebManiaConfigurationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="WebManiaConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public WebManiaConfigurationException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="WebManiaConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this error.</param>
    public WebManiaConfigurationException(
        string? message,
        Exception? innerException)
        : base(message, innerException)
    {
    }
}

