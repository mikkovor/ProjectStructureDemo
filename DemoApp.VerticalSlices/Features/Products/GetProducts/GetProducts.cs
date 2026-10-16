using DemoApp.VerticalSlices.Products;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DemoApp.VerticalSlices.Features.Products.GetProducts;

public static class GetProducts
{
    public static void MapGetProducts(this IEndpointRouteBuilder app) =>
        app.MapGet("/products", HandleAsync).WithName("GetProducts");

    private static async Task<Results<Ok<IReadOnlyList<GetProductsResponse>>, ValidationProblem>> HandleAsync(
        [AsParameters] GetProductsRequest request,
        IValidator<GetProductsRequest> validator,
        IProductRepository repository,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        var products = await repository.GetAllAsync(request.Search, request.PageSize, cancellationToken);
        IReadOnlyList<GetProductsResponse> response = products
            .Select(p => new GetProductsResponse(p.Id, p.Name, p.Price))
            .ToList();
        return TypedResults.Ok(response);
    }
}