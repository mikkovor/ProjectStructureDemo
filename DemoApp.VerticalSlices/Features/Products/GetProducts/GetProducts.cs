using DemoApp.VerticalSlices.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DemoApp.VerticalSlices.Features.Products.GetProducts;

public static class GetProducts
{
    public static void MapGetProducts(this IEndpointRouteBuilder group) =>
        group.MapGet("/", HandleAsync).WithName("GetProducts");

    private static async Task<Ok<IReadOnlyList<GetProductsResponse>>> HandleAsync(
        [AsParameters] GetProductsRequest request,
        ProductStore store,
        CancellationToken cancellationToken)
    {
        var products = await store.GetAllAsync(request.Search, request.PageSize, cancellationToken);
        IReadOnlyList<GetProductsResponse> response = products
            .Select(p => new GetProductsResponse(p.Id, p.Name, p.Price))
            .ToList();
        return TypedResults.Ok(response);
    }
}
