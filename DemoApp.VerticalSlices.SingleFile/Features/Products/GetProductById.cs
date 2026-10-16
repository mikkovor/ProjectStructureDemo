using DemoApp.VerticalSlices.SingleFile.Products;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DemoApp.VerticalSlices.SingleFile.Features.Products;

public static class GetProductById
{
    public const string EndpointName = "GetProductById";

    public sealed record Response(Guid Id, string Name, decimal Price);

    public static void MapGetProductById(this IEndpointRouteBuilder app) =>
        app.MapGet("/products/{id:guid}", HandleAsync).WithName(EndpointName);

    private static async Task<Results<Ok<Response>, NotFound>> HandleAsync(
        Guid id, IProductRepository repository, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        return product is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(new Response(product.Id, product.Name, product.Price));
    }
}
