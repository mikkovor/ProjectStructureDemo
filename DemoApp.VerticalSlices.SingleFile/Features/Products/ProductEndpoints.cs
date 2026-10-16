using DemoApp.VerticalSlices.SingleFile.Common;

namespace DemoApp.VerticalSlices.SingleFile.Features.Products;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products")
            .WithTags("Products")
            .AddEndpointFilter<ValidationFilter>()
            .ProducesValidationProblem();

        group.MapGetProducts();
        group.MapGetProductById();
        group.MapCreateProduct();
        group.MapChangePrice();
    }
}
