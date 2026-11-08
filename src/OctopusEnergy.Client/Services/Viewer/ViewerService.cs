using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Models.Viewer;

namespace OctopusEnergy.Client.Services.Viewer;

/// <summary>
/// GraphQL viewer discovery for the authenticated dashboard API key.
/// </summary>
public sealed class ViewerService : IViewerService
{
    private readonly GraphQlClient? _graphQl;

    internal ViewerService(GraphQlClient? graphQl)
    {
        _graphQl = graphQl;
    }

    /// <inheritdoc />
    public async Task<ViewerDetail> GetViewerAsync(CancellationToken cancellationToken = default)
    {
        if (_graphQl is null)
        {
            throw new OctopusEnergyRequestException(
                "A dashboard API key is required to query the viewer.");
        }

        ViewerQueryData data = await _graphQl.ExecuteAsync<ViewerQueryData>(
            ViewerQueries.GetViewer,
            variables: null,
            cancellationToken).ConfigureAwait(false);

        ViewerWire? wire = data.Viewer
            ?? throw new OctopusEnergyParseException("The viewer query returned no data.");

        return new ViewerDetail
        {
            Id = wire.Id,
            AccountNumbers = CollectDistinctAccountNumbers(wire),
        };
    }

    private static List<string> CollectDistinctAccountNumbers(ViewerWire wire)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var numbers = new List<string>();

        if (wire.Accounts is not null)
        {
            foreach (ViewerAccountWire account in wire.Accounts)
            {
                AddAccountNumber(account.Number, seen, numbers);
            }
        }

        if (wire.Portfolios is not null)
        {
            foreach (ViewerPortfolioWire portfolio in wire.Portfolios)
            {
                if (portfolio.Accounts is null)
                {
                    continue;
                }

                foreach (ViewerAccountWire account in portfolio.Accounts)
                {
                    AddAccountNumber(account.Number, seen, numbers);
                }
            }
        }

        return numbers;
    }

    private static void AddAccountNumber(string? number, HashSet<string> seen, List<string> numbers)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            return;
        }

        if (seen.Add(number))
        {
            numbers.Add(number);
        }
    }
}
