using DemoApp.CleanArchitecture.Api.Filters;
using DemoApp.CleanArchitecture.Api.Requests;
using DemoApp.CleanArchitecture.Api.Responses;
using DemoApp.CleanArchitecture.Application.Products.ChangePrice;
using DemoApp.CleanArchitecture.Application.Products.CreateProduct;
using DemoApp.CleanArchitecture.Application.Products.GetProductById;
using DemoApp.CleanArchitecture.Application.Products.GetProducts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DemoApp.CleanArchitecture.Api.Endpoints;

public static class ProductsEndpoints
{
    private const string GetProductByIdName = "GetProductById";

    public static void MapProducts(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products")
            .WithTags("Products")
            .AddEndpointFilter<ValidationFilter>()
            .ProducesValidationProblem();

        group.MapGet("/", GetProductsAsync).WithName("GetProducts");
        group.MapGet("/{id:guid}", GetProductByIdAsync).WithName(GetProductByIdName);
        group.MapPost("/", CreateProductAsync).WithName("CreateProduct");
        group.MapPut("/{id:guid}/price", ChangePriceAsync).WithName("ChangePrice");
    }

    private static async Task<Ok<IReadOnlyList<ProductResponse>>> GetProductsAsync(
        [AsParameters] GetProductsRequest request,
        GetProductsHandler handler,
        CancellationToken cancellationToken)
    {
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

    private static async Task<CreatedAtRoute<ProductResponse>> CreateProductAsync(
        CreateProductRequest request,
        CreateProductHandler handler,
        CancellationToken cancellationToken)
    {
        var product = await handler.HandleAsync(new CreateProductCommand(request.Name, request.Price), cancellationToken);
        return TypedResults.CreatedAtRoute(ProductResponse.From(product), GetProductByIdName, new { id = product.Id });
    }

    private static async Task<Results<Ok<ProductResponse>, NotFound, ProblemHttpResult>> ChangePriceAsync(
        Guid id,
        ChangePriceRequest request,
        ChangePriceHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new ChangePriceCommand(id, request.Price), cancellationToken);
        return result switch
        {
            ChangePriceResult.Changed changed => TypedResults.Ok(ProductResponse.From(changed.Product)),
            ChangePriceResult.NotFound => TypedResults.NotFound(),
            _ => TypedResults.Problem(
                title: "Price increase too large",
                detail: "A price may not be increased by more than 50% at once.",
                statusCode: StatusCodes.Status422UnprocessableEntity)
        };
    }
}
