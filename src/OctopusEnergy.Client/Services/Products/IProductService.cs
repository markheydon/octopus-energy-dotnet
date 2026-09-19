using OctopusEnergy.Client.Models.Products;

namespace OctopusEnergy.Client.Services.Products;

/// <summary>
/// Product catalogue and product detail operations.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Lists energy products, following REST pagination automatically.
    /// </summary>
    /// <param name="request">Optional filters documented by Octopus.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>All products across pages.</returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="request"/>.<see cref="ProductListRequest.Brand"/> is empty or whitespace.
    /// </exception>
    /// <exception cref="OctopusEnergyApiException">
    /// Thrown when the API returns an error response during pagination.
    /// </exception>
    /// <exception cref="OctopusEnergyHttpException">
    /// Thrown when the API returns a non-success HTTP status without a parseable error body.
    /// </exception>
    /// <remarks>
    /// Public catalogue endpoints do not require authentication. Do not assume a single brand on the UK host.
    /// </remarks>
    IAsyncEnumerable<Product> ListAsync(
        ProductListRequest? request = null,
        CancellationToken cancellationToken = default);

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
    /// <exception cref="OctopusEnergyApiException">
    /// Thrown when the API returns an error response (for example HTTP 404 for an unknown product code).
    /// </exception>
    /// <exception cref="OctopusEnergyHttpException">
    /// Thrown when the API returns a non-success HTTP status without a parseable error body.
    /// </exception>
    Task<ProductDetail> GetAsync(
        string productCode,
        DateTimeOffset? tariffsActiveAt = null,
        CancellationToken cancellationToken = default);
}
