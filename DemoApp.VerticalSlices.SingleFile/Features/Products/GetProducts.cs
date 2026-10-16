using DemoApp.VerticalSlices.SingleFile.Data;
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

    public static void MapGetProducts(this IEndpointRouteBuilder group) =>
        group.MapGet("/", HandleAsync).WithName("GetProducts");

    private static async Task<Ok<IReadOnlyList<Response>>> HandleAsync(
        [AsParameters] Query query,
        ProductStore store,
        CancellationToken cancellationToken)
    {
        var products = await store.GetAllAsync(query.Search, query.PageSize, cancellationToken);
        IReadOnlyList<Response> response = products
            .Select(p => new Response(p.Id, p.Name, p.Price))
            .ToList();
        return TypedResults.Ok(response);
    }
}
