using DemoApp.VerticalSlices.Features.Products.GetProductById;
using DemoApp.VerticalSlices.Products;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DemoApp.VerticalSlices.Features.Products.CreateProduct;

public static class CreateProduct
{
    public static void MapCreateProduct(this IEndpointRouteBuilder app) =>
        app.MapPost("/products", HandleAsync).WithName("CreateProduct");

    private static async Task<Results<CreatedAtRoute<CreateProductResponse>, ValidationProblem>> HandleAsync(
        CreateProductRequest request,
        IValidator<CreateProductRequest> validator,
        IProductRepository repository,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        var product = await repository.AddAsync(request.Name, request.Price, cancellationToken);
        var response = new CreateProductResponse(product.Id, product.Name, product.Price);
        return TypedResults.CreatedAtRoute(response, "GetProductById", new { id = product.Id });
    }
}