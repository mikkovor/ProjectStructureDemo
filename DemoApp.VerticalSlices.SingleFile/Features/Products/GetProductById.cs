using DemoApp.VerticalSlices.SingleFile.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DemoApp.VerticalSlices.SingleFile.Features.Products;

public static class GetProductById
{
    public const string EndpointName = "GetProductById";

    public sealed record Response(Guid Id, string Name, decimal Price);

    public static void MapGetProductById(this IEndpointRouteBuilder group) =>
        group.MapGet("/{id:guid}", HandleAsync).WithName(EndpointName);

    private static async Task<Results<Ok<Response>, NotFound>> HandleAsync(
        Guid id, ProductStore store, CancellationToken cancellationToken)
    {
        var product = await store.GetByIdAsync(id, cancellationToken);
        return product is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(new Response(product.Id, product.Name, product.Price));
    }
}
