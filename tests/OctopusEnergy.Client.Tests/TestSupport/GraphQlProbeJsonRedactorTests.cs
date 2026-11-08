namespace OctopusEnergy.Client.Tests.TestSupport;

public sealed class GraphQlProbeJsonRedactorTests
{
    [Fact]
    public void RedactAndTruncate_ReplacesSensitivePatterns()
    {
        const string input = """
            {
              "data": {
                "viewer": { "id": "12345678", "email": "customer@octopus.energy" },
                "account": { "number": "A-REALACCT1" }
              },
              "mpan": "1000123456789",
              "mprn": "9876543210",
              "serialNumber": "SN-SECRET",
              "token": "eyJhbGciOiJIUzI1NiJ9.abc.def",
              "postcode": "SW1A 1AA"
            }
            """;

        string redacted = GraphQlProbeJsonRedactor.RedactAndTruncate(input);

        Assert.Contains("viewer-id-redacted", redacted, StringComparison.Ordinal);
        Assert.Contains("A-TEST0001", redacted, StringComparison.Ordinal);
        Assert.Contains("1000000000001", redacted, StringComparison.Ordinal);
        Assert.Contains("\"mprn\":\"1234567890\"", redacted, StringComparison.Ordinal);
        Assert.Contains("TEST-SERIAL-RED", redacted, StringComparison.Ordinal);
        Assert.Contains("eyJhbGciOiJub25lIn0.redacted.redacted", redacted, StringComparison.Ordinal);
        Assert.Contains("user@example.test", redacted, StringComparison.Ordinal);
        Assert.Contains("W1 1AA", redacted, StringComparison.Ordinal);
        Assert.DoesNotContain("customer@octopus.energy", redacted, StringComparison.Ordinal);
        Assert.DoesNotContain("A-REALACCT1", redacted, StringComparison.Ordinal);
        Assert.DoesNotContain("1000123456789", redacted, StringComparison.Ordinal);
        Assert.DoesNotContain("SN-SECRET", redacted, StringComparison.Ordinal);
    }

    [Fact]
    public void RedactAndTruncate_WhenExceedsMaxLength_AppendsTruncationMarker()
    {
        string input = new('x', 2_148);

        string redacted = GraphQlProbeJsonRedactor.RedactAndTruncate(input);

        Assert.EndsWith("…(truncated)", redacted, StringComparison.Ordinal);
        Assert.True(redacted.Length < input.Length);
    }
}
