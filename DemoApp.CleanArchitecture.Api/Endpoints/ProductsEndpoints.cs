using DemoApp.CleanArchitecture.Api.Requests;
using DemoApp.CleanArchitecture.Api.Responses;
using DemoApp.CleanArchitecture.Application.Products.CreateProduct;
using DemoApp.CleanArchitecture.Application.Products.GetProductById;
using DemoApp.CleanArchitecture.Application.Products.GetProducts;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DemoApp.CleanArchitecture.Api.Endpoints;

public static class ProductsEndpoints
{
    public static void MapProducts(this IEndpointRouteBuilder app)
    {
        app.MapGet("/products", GetProductsAsync).WithName("GetProducts");
        app.MapGet("/products/{id:guid}", GetProductByIdAsync).WithName("GetProductById");
        app.MapPost("/products", CreateProductAsync).WithName("CreateProduct");
    }

    private static async Task<Results<Ok<IReadOnlyList<ProductResponse>>, ValidationProblem>> GetProductsAsync(
        [AsParameters] GetProductsRequest request,
        IValidator<GetProductsRequest> validator,
        GetProductsHandler handler,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        var products = await handler.HandleAsync(new GetProductsQuery(request.Search, request.PageSize), cancellationToken);
        IReadOnlyList<ProductResponse> response = products.Select(ProductResponse.From).ToList();
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ProductResponse>, NotFound>> GetProductByIdAsync(
        Guid id,
        GetProductByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetProductByIdQuery(id), cancellationToken);
        return result switch
        {
            GetProductByIdResult.Found found => TypedResults.Ok(ProductResponse.From(found.Product)),
            _ => TypedResults.NotFound()
        };
    }

    private static async Task<Results<CreatedAtRoute<ProductResponse>, ValidationProblem>> CreateProductAsync(
        CreateProductRequest request,
        IValidator<CreateProductRequest> validator,
        CreateProductHandler handler,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        var product = await handler.HandleAsync(new CreateProductCommand(request.Name, request.Price), cancellationToken);
        return TypedResults.CreatedAtRoute(ProductResponse.From(product), "GetProductById", new { id = product.Id });
    }
}
