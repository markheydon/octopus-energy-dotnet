namespace OctopusEnergy.Client;

/// <summary>
/// Defaults for account billing GraphQL list operations (issue #23).
/// </summary>
public static class AccountBillingListDefaults
{
    /// <summary>
    /// Default <c>first</c> page size when callers omit an explicit value (stays under complexity 200).
    /// </summary>
    public const int DefaultFirst = 25;
}
