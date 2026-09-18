namespace OctopusEnergy.Client.Tests.TestSupport;

/// <summary>
/// Guards the recorded-fixture test strategy used in CI.
/// </summary>
public sealed class RecordedFixturesTests
{
    [Fact]
    public void FixtureFile_ReadsCheckedInJson()
    {
        string content = FixtureFile.Read("products-list-page-1.json");

        Assert.Contains("AGILE-FLEX-22-11-25", content, StringComparison.Ordinal);
        Assert.DoesNotContain("sk_live", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Bearer ", content, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveTestGate_WhenUnset_IsDisabled()
    {
        string? previous = Environment.GetEnvironmentVariable(LiveTestGate.EnableEnvironmentVariable);

        try
        {
            Environment.SetEnvironmentVariable(LiveTestGate.EnableEnvironmentVariable, null);
            Assert.False(LiveTestGate.IsEnabled);
        }
        finally
        {
            Environment.SetEnvironmentVariable(LiveTestGate.EnableEnvironmentVariable, previous);
        }
    }
}
