using DemoApp.VerticalSlices.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DemoApp.VerticalSlices.Features.Products.ChangePrice;

public static class ChangePrice
{
    public static void MapChangePrice(this IEndpointRouteBuilder group) =>
        group.MapPut("/{id:guid}/price", HandleAsync).WithName("ChangePrice");

    private static async Task<Results<Ok<ChangePriceResponse>, NotFound, ProblemHttpResult>> HandleAsync(
        Guid id,
        ChangePriceRequest request,
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
        return TypedResults.Ok(new ChangePriceResponse(product.Id, product.Name, product.Price));
    }
}
