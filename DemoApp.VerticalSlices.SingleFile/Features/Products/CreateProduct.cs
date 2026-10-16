using DemoApp.VerticalSlices.SingleFile.Data;
using DemoApp.VerticalSlices.SingleFile.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DemoApp.VerticalSlices.SingleFile.Features.Products;

public static class CreateProduct
{
    public sealed record Request(string Name, decimal Price);

    public sealed record Response(Guid Id, string Name, decimal Price);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(Product.MaxNameLength);
            RuleFor(x => x.Price).GreaterThan(0);
        }
    }

    public static void MapCreateProduct(this IEndpointRouteBuilder group) =>
        group.MapPost("/", HandleAsync).WithName("CreateProduct");

    private static async Task<CreatedAtRoute<Response>> HandleAsync(
        Request request,
        ProductStore store,
        CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Price);
        await store.AddAsync(product, cancellationToken);
        var response = new Response(product.Id, product.Name, product.Price);
        return TypedResults.CreatedAtRoute(response, GetProductById.EndpointName, new { id = product.Id });
    }
}
