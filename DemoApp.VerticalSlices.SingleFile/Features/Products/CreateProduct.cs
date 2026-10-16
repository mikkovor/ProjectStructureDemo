using DemoApp.VerticalSlices.SingleFile.Products;
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
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Price).GreaterThan(0);
        }
    }

    public static void MapCreateProduct(this IEndpointRouteBuilder app) =>
        app.MapPost("/products", HandleAsync).WithName("CreateProduct");

    private static async Task<Results<CreatedAtRoute<Response>, ValidationProblem>> HandleAsync(
        Request request,
        IValidator<Request> validator,
        IProductRepository repository,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        var product = await repository.AddAsync(request.Name, request.Price, cancellationToken);
        var response = new Response(product.Id, product.Name, product.Price);
        return TypedResults.CreatedAtRoute(response, GetProductById.EndpointName, new { id = product.Id });
    }
}
