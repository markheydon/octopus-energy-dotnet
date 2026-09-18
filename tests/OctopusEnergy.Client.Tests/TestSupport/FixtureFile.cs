namespace OctopusEnergy.Client.Tests.TestSupport;

internal static class FixtureFile
{
    internal static string Read(string relativePath)
    {
        string fullPath = Path.Combine(AppContext.BaseDirectory, "TestSupport", "Fixtures", relativePath);
        return File.ReadAllText(fullPath);
    }
}
