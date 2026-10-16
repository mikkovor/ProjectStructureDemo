using DemoApp.VerticalSlices.SingleFile.Data;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DemoApp.VerticalSlices.SingleFile.Features.Products;

public static class ChangePrice
{
    public sealed record Request(decimal Price);

    public sealed record Response(Guid Id, string Name, decimal Price);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Price).GreaterThan(0);
        }
    }

    public static void MapChangePrice(this IEndpointRouteBuilder group) =>
        group.MapPut("/{id:guid}/price", HandleAsync).WithName("ChangePrice");

    private static async Task<Results<Ok<Response>, NotFound, ProblemHttpResult>> HandleAsync(
        Guid id,
        Request request,
        ProductStore store,
        CancellationToken cancellationToken)
    {
        var product = await store.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return TypedResults.NotFound();
        }

        // The rule is owned by the domain entity; the slice only orchestrates.
        if (!product.TryChangePrice(request.Price))
        {
            return TypedResults.Problem(
                title: "Price increase too large",
                detail: "A price may not be increased by more than 50% at once.",
                statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        await store.UpdateAsync(product, cancellationToken);
        return TypedResults.Ok(new Response(product.Id, product.Name, product.Price));
    }
}
