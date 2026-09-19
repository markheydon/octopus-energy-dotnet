namespace OctopusEnergy.Client;

/// <summary>
/// Thrown when a successful HTTP response body cannot be deserialised into the expected model.
/// </summary>
public sealed class OctopusEnergyParseException : OctopusEnergyException
{
    /// <summary>
    /// Creates an exception with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public OctopusEnergyParseException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates an exception with the specified message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public OctopusEnergyParseException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
