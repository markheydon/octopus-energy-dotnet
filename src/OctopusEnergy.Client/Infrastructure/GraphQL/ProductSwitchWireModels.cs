using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal sealed class InitiateProductSwitchMutationData
{
    [JsonPropertyName("initiateProductSwitch")]
    public InitiateProductSwitchWire? InitiateProductSwitch { get; init; }
}

internal sealed class InitiateProductSwitchWire
{
    [JsonPropertyName("productCode")]
    public string? ProductCode { get; init; }

    [JsonPropertyName("switchDate")]
    public string? SwitchDate { get; init; }
}

internal sealed class InitiateProductSwitchMutationVariables
{
    [JsonPropertyName("accountNumber")]
    public required string AccountNumber { get; init; }

    [JsonPropertyName("switchDate")]
    public required string SwitchDate { get; init; }

    [JsonPropertyName("quotedProductId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? QuotedProductId { get; init; }

    [JsonPropertyName("offerIdentifier")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OfferIdentifier { get; init; }

    [JsonPropertyName("suppressCommunications")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SuppressCommunications { get; init; }
}
