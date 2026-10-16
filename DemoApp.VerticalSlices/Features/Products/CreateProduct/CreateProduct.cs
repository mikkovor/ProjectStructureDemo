using DemoApp.VerticalSlices.Data;
using DemoApp.VerticalSlices.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DemoApp.VerticalSlices.Features.Products.CreateProduct;

public static class CreateProduct
{
    public static void MapCreateProduct(this IEndpointRouteBuilder group) =>
        group.MapPost("/", HandleAsync).WithName("CreateProduct");

    private static async Task<CreatedAtRoute<CreateProductResponse>> HandleAsync(
        CreateProductRequest request,
        ProductStore store,
        CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Price);
        await store.AddAsync(product, cancellationToken);
        var response = new CreateProductResponse(product.Id, product.Name, product.Price);
        return TypedResults.CreatedAtRoute(response, GetProductById.GetProductById.EndpointName, new { id = product.Id });
    }
}
