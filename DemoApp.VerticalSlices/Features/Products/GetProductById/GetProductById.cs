using DemoApp.VerticalSlices.Products;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DemoApp.VerticalSlices.Features.Products.GetProductById;

public static class GetProductById
{
    public const string EndpointName = "GetProductById";

    public static void MapGetProductById(this IEndpointRouteBuilder app) =>
        app.MapGet("/products/{id:guid}", HandleAsync).WithName(EndpointName);

    private static async Task<Results<Ok<ProductResponse>, NotFound>> HandleAsync(
        Guid id, IProductRepository repository, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        return product is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(new ProductResponse(product.Id, product.Name, product.Price));
    }
}