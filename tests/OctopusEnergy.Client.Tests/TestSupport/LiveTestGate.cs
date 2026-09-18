namespace OctopusEnergy.Client.Tests.TestSupport;

/// <summary>
/// Opt-in gate for tests that call the live Octopus API. Default CI and local
/// <c>dotnet test</c> runs use recorded fixtures only.
/// </summary>
internal static class LiveTestGate
{
    /// <summary>
    /// Environment variable that must be set to <c>1</c> or <c>true</c> to run live API tests.
    /// </summary>
    internal const string EnableEnvironmentVariable = "OCTOPUS_ENERGY_ENABLE_LIVE_TESTS";

    /// <summary>
    /// Optional API key for live tests. Required when live tests are enabled.
    /// </summary>
    internal const string ApiKeyEnvironmentVariable = "OCTOPUS_ENERGY_API_KEY";

    internal static bool IsEnabled
    {
        get
        {
            string? value = Environment.GetEnvironmentVariable(EnableEnvironmentVariable);
            return string.Equals(value, "1", StringComparison.Ordinal)
                || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }
    }

    internal static void SkipUnlessEnabled()
    {
        if (!IsEnabled)
        {
            Assert.Skip(
                $"Live API tests are disabled. Set {EnableEnvironmentVariable}=1 to opt in. " +
                "CI and default local runs use recorded fixtures only.");
        }
    }
}
