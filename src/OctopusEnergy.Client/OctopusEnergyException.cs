namespace OctopusEnergy.Client;

/// <summary>
/// Base exception for Octopus Energy SDK failures.
/// </summary>
public class OctopusEnergyException : Exception
{
    /// <summary>
    /// Creates an exception with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public OctopusEnergyException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates an exception with the specified message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public OctopusEnergyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
