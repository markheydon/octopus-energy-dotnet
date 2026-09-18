using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Accounts;

/// <summary>
/// Customer account including properties and meter points.
/// </summary>
public sealed class Account
{
    private IReadOnlyList<AccountProperty> _properties = [];

    /// <summary>
    /// Account number (for example <c>A-12345678</c>).
    /// </summary>
    [JsonPropertyName("number")]
    public string Number { get; init; } = string.Empty;

    /// <summary>
    /// Properties linked to the account.
    /// </summary>
    [JsonPropertyName("properties")]
    public IReadOnlyList<AccountProperty> Properties
    {
        get => _properties;
        init => _properties = value ?? [];
    }
}
