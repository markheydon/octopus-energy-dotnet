namespace OctopusEnergy.Client;

/// <summary>
/// Thrown when a request violates a documented local API contract constraint
/// before any HTTP call is made.
/// </summary>
public sealed class OctopusEnergyRequestException : OctopusEnergyException
{
    /// <summary>
    /// Creates an exception with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public OctopusEnergyRequestException(string message)
        : base(message)
    {
    }
}
