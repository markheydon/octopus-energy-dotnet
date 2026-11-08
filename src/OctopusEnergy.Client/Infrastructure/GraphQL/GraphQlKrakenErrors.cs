namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class GraphQlKrakenErrors
{
    internal const string ComplexityErrorCode = "KT-CT-1188";
    internal const string NodeLimitErrorCode = "KT-CT-1189";
    internal const string FieldRateLimitErrorCode = "KT-CT-1199";

    internal static OctopusEnergyKrakenException Map(IReadOnlyList<GraphQlError> errors)
    {
        GraphQlError first = errors[0];
        string? code = first.Extensions?.ErrorCode;
        string? message = first.Message;

        if (code is null)
        {
            string fallbackMessage = message ?? "The GraphQL API returned an error.";
            return new OctopusEnergyKrakenException(string.Empty, fallbackMessage);
        }

        return code switch
        {
            ComplexityErrorCode => new OctopusEnergyKrakenComplexityException(code, message),
            NodeLimitErrorCode => new OctopusEnergyKrakenNodeLimitException(code, message),
            FieldRateLimitErrorCode => new OctopusEnergyKrakenFieldRateLimitException(code, message),
            _ => new OctopusEnergyKrakenException(code, message),
        };
    }
}
