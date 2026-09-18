using OctopusEnergy.Client.Infrastructure.Http;
using OctopusEnergy.Client.Models.Products;

namespace OctopusEnergy.Client.Services.Products;

/// <summary>
/// Product catalogue and product detail operations.
/// </summary>
public sealed class ProductService
{
    private const string ProductsPath = "products/";

    private readonly RestClient _rest;

    internal ProductService(RestClient rest)
    {
        ArgumentNullException.ThrowIfNull(rest);
        _rest = rest;
    }

    /// <summary>
    /// Lists energy products, following REST pagination automatically.
    /// </summary>
    /// <param name="request">Optional filters documented by Octopus.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>All products across pages.</returns>
    /// <remarks>
    /// Public catalogue endpoints do not require authentication. Do not assume a single brand on the UK host.
    /// </remarks>
    public IAsyncEnumerable<Product> ListAsync(
        ProductListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        string path = BuildListPath(request);
        return _rest.GetAllPagesAsync<Product>(path, cancellationToken);
    }

    /// <summary>
    /// Retrieves a single product including tariffs by GSP and payment method.
    /// </summary>
    /// <param name="productCode">Product code (for example <c>AGILE-FLEX-22-11-25</c>).</param>
    /// <param name="tariffsActiveAt">
    /// Optional instant for tariff snapshots. When omitted, the API returns tariffs active now.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Product detail.</returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="productCode"/> is null or whitespace.
    /// </exception>
    public Task<ProductDetail> GetAsync(
        string productCode,
        DateTimeOffset? tariffsActiveAt = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new OctopusEnergyRequestException("Product code is required.");
        }

        string path = BuildDetailPath(productCode, tariffsActiveAt);
        return _rest.GetAsync<ProductDetail>(path, cancellationToken);
    }

    private static string BuildListPath(ProductListRequest? request)
    {
        if (request is null)
        {
            return ProductsPath;
        }

        List<RestQuery.QueryParameter> parameters = new();

        if (!string.IsNullOrWhiteSpace(request.Brand))
        {
            parameters.Add(new RestQuery.QueryParameter("brand", request.Brand));
        }

        if (request.IsVariable is not null)
        {
            parameters.Add(new RestQuery.QueryParameter(
                "is_variable",
                FormatBoolean(request.IsVariable.Value)));
        }

        if (request.IsGreen is not null)
        {
            parameters.Add(new RestQuery.QueryParameter(
                "is_green",
                FormatBoolean(request.IsGreen.Value)));
        }

        if (request.IsTracker is not null)
        {
            parameters.Add(new RestQuery.QueryParameter(
                "is_tracker",
                FormatBoolean(request.IsTracker.Value)));
        }

        if (request.IsPrepay is not null)
        {
            parameters.Add(new RestQuery.QueryParameter(
                "is_prepay",
                FormatBoolean(request.IsPrepay.Value)));
        }

        if (request.IsBusiness is not null)
        {
            parameters.Add(new RestQuery.QueryParameter(
                "is_business",
                FormatBoolean(request.IsBusiness.Value)));
        }

        if (request.AvailableAt is not null)
        {
            parameters.Add(new RestQuery.QueryParameter(
                "available_at",
                RestQuery.FormatDateTimeOffset(request.AvailableAt.Value)));
        }

        return RestQuery.Append(ProductsPath, parameters);
    }

    private static string BuildDetailPath(string productCode, DateTimeOffset? tariffsActiveAt)
    {
        string path = $"products/{Uri.EscapeDataString(productCode)}/";

        if (tariffsActiveAt is null)
        {
            return path;
        }

        List<RestQuery.QueryParameter> parameters =
        [
            new RestQuery.QueryParameter(
                "tariffs_active_at",
                RestQuery.FormatDateTimeOffset(tariffsActiveAt.Value)),
        ];

        return RestQuery.Append(path, parameters);
    }

    private static string FormatBoolean(bool value)
    {
        return value.ToString().ToLowerInvariant();
    }
}
