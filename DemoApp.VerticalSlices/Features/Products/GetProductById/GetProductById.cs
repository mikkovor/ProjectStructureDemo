using DemoApp.VerticalSlices.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DemoApp.VerticalSlices.Features.Products.GetProductById;

public static class GetProductById
{
    public const string EndpointName = "GetProductById";

    public static void MapGetProductById(this IEndpointRouteBuilder group) =>
        group.MapGet("/{id:guid}", HandleAsync).WithName(EndpointName);

    private static async Task<Results<Ok<GetProductByIdResponse>, NotFound>> HandleAsync(
        Guid id, ProductStore store, CancellationToken cancellationToken)
    {
        var product = await store.GetByIdAsync(id, cancellationToken);
        return product is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(new GetProductByIdResponse(product.Id, product.Name, product.Price));
    }
}
