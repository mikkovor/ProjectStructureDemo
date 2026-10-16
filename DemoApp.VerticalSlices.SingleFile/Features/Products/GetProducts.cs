using DemoApp.VerticalSlices.SingleFile.Products;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DemoApp.VerticalSlices.SingleFile.Features.Products;

public static class GetProducts
{
    public sealed record Query(string? Search, int? PageSize);

    public sealed record Response(Guid Id, string Name, decimal Price);

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Search).MaximumLength(100);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100).When(x => x.PageSize.HasValue);
        }
    }

    public static void MapGetProducts(this IEndpointRouteBuilder app) =>
        app.MapGet("/products", HandleAsync).WithName("GetProducts");

    private static async Task<Results<Ok<IReadOnlyList<Response>>, ValidationProblem>> HandleAsync(
        [AsParameters] Query query,
        IValidator<Query> validator,
        IProductRepository repository,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        var products = await repository.GetAllAsync(query.Search, query.PageSize, cancellationToken);
        IReadOnlyList<Response> response = products
            .Select(p => new Response(p.Id, p.Name, p.Price))
            .ToList();
        return TypedResults.Ok(response);
    }
}
